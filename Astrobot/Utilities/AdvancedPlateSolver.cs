using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Helpers;
using DiscordAstroBot.Objects.Simbad;
using SkiaSharp;

namespace DiscordAstroBot.Utilities
{
    /// <summary>
    /// AdvancePlateSolver that marks more custom objects on a plate solved image, using
    /// SIMBAD to get the objects and WCS coordinate transformations from astropy to 
    /// map the objects onto the image
    /// </summary>
    public static class AdvancedPlateSolver
    {
        /// <summary>
        /// Marks all the objects on the image
        /// </summary>
        /// <param name="plateSolvedImage"></param>
        /// <param name="astrometryResult"></param>
        /// <returns></returns>
        public static PlateSolvedAndMarkedImage MarkObjectsOnImage(SKBitmap plateSolvedImage,
            AstrometrySubmissionResult astrometryResult)
        {
            // First, lets find all the objects that are inside the picture
            var objects = GetObjectsInImage(astrometryResult.CalibrationData);

            // Export objects to csv file
            ExportObjectsRADECCoordsToFile(
                objects,
                Path.Combine(
                    new[] {
                        ConfigurationManager.AppSettings["AstropyWCSConverterPath"],
                        "WCS_Transform.csv"
                    }));

            // Download the wcs.fit file and save it to disk
            DownloadAndSaveWCSFitFile(
                astrometryResult,
                 Path.Combine(
                    new[] {
                        ConfigurationManager.AppSettings["AstropyWCSConverterPath"],
                        "wcs.fits"
                    }));

            // Run the Python script to transform the coordinates
            TransformRADECToXYCoords();

            // Read output CSV from the python script
            var transformedObjects = LoadTransformedObjets(Path.Combine(new[] {
                        ConfigurationManager.AppSettings["AstropyWCSConverterPath"],
                        "WCS_Transform_Output.csv"
                    }), objects);

            // Mark the objects on the image
            MarkObjectsOnImage(plateSolvedImage, transformedObjects, astrometryResult.CalibrationData);

            // Get CSV for objects
            var csv = CreateCSV(transformedObjects);

            return new PlateSolvedAndMarkedImage() { MappedObjectsInImage = transformedObjects, MarkedImage = plateSolvedImage, InfoCSV = csv };
        }

        /// <summary>
        /// Gets all the objects that are inside the image from SIMBAD
        /// </summary>
        /// <param name="calibrationData"></param>
        /// <returns></returns>
        private static List<AstronomicalObjectInfo> GetObjectsInImage(AstrometrySubmissionCalibrationData calibrationData)
        {
            // Query SIMBAD for all the objects inside the image
            // Use Radius * 1.8 to make sure to catch every object even if its at the edge of a non-quadratic image
            var objects = Mappers.Simbad.SimbadQuery.QueryAround(
                new RADECCoords() { DEC = calibrationData.DEC, RA = calibrationData.RA },
                calibrationData.Radius * 3.0f);

            var result = new List<AstronomicalObjectInfo>();

            foreach (var obj in objects.Where(x => x.Sections.Count > 0))
            {
                var parsedObj = AstronomicalObjectInfo.FromSimbadResult(obj);

                // Ignore objects with no coordinates (why am I receiving those either way?)
                if (parsedObj.Coordinates == null) continue;

                result.Add(parsedObj);
            }

            return result;
        }

        /// <summary>
        /// Exports the RA/DEC-Coordinates to a CSV file
        /// </summary>
        /// <param name="objects"></param>
        private static void ExportObjectsRADECCoordsToFile(List<AstronomicalObjectInfo> objects, string file)
        {
            var csvFile = objects.Aggregate(string.Empty, (current, obj) => current + $"{obj.Name};{obj.Coordinates.RA};{obj.Coordinates.DEC}\r\n");
            File.WriteAllText(file, csvFile);
        }

        /// <summary>
        /// Downloads and saves the WCSFit file for the astropy script to transform the coordinates
        /// </summary>
        /// <param name="plateSolveResult"></param>
        /// <param name="target"></param>
        private static void DownloadAndSaveWCSFitFile(AstrometrySubmissionResult plateSolveResult, string targetFile)
        {
            // Get the file from the server
            using (var memoryStream = AstrometryHelper.DownloadWCSFitsFile(plateSolveResult.JobID))
            {
                using (var fs = File.Create(targetFile))
                {
                    // Save to file to disk
                    memoryStream.WriteTo(fs);
                }
            }
        }

        /// <summary>
        /// Call the python script to transform the coordinates
        /// </summary>
        private static void TransformRADECToXYCoords()
        {
            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    WorkingDirectory = ConfigurationManager.AppSettings["AstropyWCSConverterPath"],
                    FileName = "cmd",
                    Arguments = "/c python " + Path.Combine(new[] { ConfigurationManager.AppSettings["AstropyWCSConverterPath"], "AstropyWCSConverter.py" }),
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    CreateNoWindow = false,
                },
            };

            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Load the transformed coordinates from the generated CSV file
        /// </summary>
        /// <returns></returns>
        private static List<MappedAstroObject> LoadTransformedObjets(string file, List<AstronomicalObjectInfo> objects)
        {
            var fileText = new StreamReader(File.OpenRead(file));
            var res = new List<MappedAstroObject>();

            while (!fileText.EndOfStream)
            {
                var line = fileText.ReadLine();
                var columns = line.Split(new[] { ';' });
                res.Add(new MappedAstroObject()
                {
                    AstroObject = objects.FirstOrDefault(x => x.Name.Trim() == columns[0].Trim()),
                    X = Convert.ToSingle(columns[3]),
                    Y = Convert.ToSingle(columns[4])
                });
            }

            fileText.Dispose();

            return res;
        }

        /// <summary>
        /// Marks the given objects on the image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="objects"></param>
        private static void MarkObjectsOnImage(SKBitmap image, List<MappedAstroObject> objects, AstrometrySubmissionCalibrationData calibrationData)
        {
            foreach (var obj in objects.Where(x => x.AstroObject != null))
            {
                // If the object has known dimensions, add a ellipse around its
                if (obj.AstroObject.AngularDimension != null)
                {
                    var radius = new[] { obj.AstroObject.AngularDimension.XSize / (calibrationData.PixScale / 60), obj.AstroObject.AngularDimension.YSize / (calibrationData.PixScale / 60) }.Max() / 2;

                    ImageUtility.AddEllipse(
                        image,
                        obj.X,
                        obj.Y,
                        radius,
                        radius,
                        +calibrationData.Orientation - obj.AstroObject.AngularDimension.Rotation,
                        image.Width / 1750);                    

                    ImageUtility.AddLabel(image, Convert.ToInt32(obj.X + (Math.Sqrt(2) / 2 * radius * 1.2) + image.Width*0.0002f), Convert.ToInt32(obj.Y + (Math.Sqrt(2) / 2 * radius * 1.2) + image.Width * 0.0002f), 30 + (image.Width / 180), true, obj.AstroObject.Name);
                }
                // Per default mark objects with a crosshair + label
                else
                {
                    var sizeSmallFont = 20 + (image.Width / 550);
                    var sizeLargeFont = 30 + (image.Width / 150);
                    var lineWidthSmall = 0.75f + (image.Width / 3000f);
                    var lineWidthLarge = 1.25f + (image.Width / 1000f);

                    var isSmall = true;
                    if (obj.AstroObject.Name.StartsWith("HD"))
                        isSmall = false;

                    var bold = !isSmall;

                    ImageUtility.AddCrossMarker(image, Convert.ToInt32(obj.X), Convert.ToInt32(obj.Y), isSmall ? lineWidthSmall : lineWidthLarge);
                    ImageUtility.AddLabel(image, Convert.ToInt32(obj.X + 0.0025 * image.Width), Convert.ToInt32(obj.Y + 0.0025 * image.Height), isSmall ? sizeSmallFont : sizeLargeFont, bold, obj.AstroObject.Name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static byte[] CreateCSV(List<MappedAstroObject> objects)
        {
            var csvString = "Object;Type;Angular Size X [arcmin];Angular Size Y [arcmin];Distance;Distance Unit;Distance Error;Distance measurement method;Distance measurement reference;Redshift (z);Velocity [km/s];Error Velocity or z;RA;DEC;X;Y\r\n";

            foreach (var obj in objects)
            {
                if (obj.AstroObject == null)
                    continue;

                csvString += $"{obj.AstroObject.Name};"+
                             $"{obj.AstroObject.ObjectType};" +
                             $"{obj.AstroObject.AngularDimension?.XSize};" +
                             $"{obj.AstroObject.AngularDimension?.YSize};" +
                             $"{obj.AstroObject.DistanceMeasurements.FirstOrDefault()?.Distance};" +
                             $"{obj.AstroObject.DistanceMeasurements.FirstOrDefault()?.Unit};" +
                             $"{obj.AstroObject.DistanceMeasurements.FirstOrDefault()?.ErrMinus};" +
                             $"{obj.AstroObject.DistanceMeasurements.FirstOrDefault()?.Method};" +
                             $"{obj.AstroObject.DistanceMeasurements.FirstOrDefault()?.Reference};" +
                             $"{obj.AstroObject.RadialVelocity.Redshift};" +
                             $"{obj.AstroObject.RadialVelocity.Velocity};" +
                             $"{(!string.IsNullOrEmpty(obj.AstroObject.RadialVelocity.Velocity) ? obj.AstroObject.RadialVelocity.Error : string.Empty)};" +
                             $"{obj.AstroObject.Coordinates.RA};" +
                             $"{obj.AstroObject.Coordinates.DEC};" +
                             $"{obj.X};" +
                             $"{obj.Y}".Replace("\r", "").Replace("\n", "")  + "\r\n";
            }

            return Encoding.ASCII.GetBytes(csvString);
        }
    }

    /// <summary>
    /// Value object to describe a mapped object on the image
    /// </summary>
    public class MappedAstroObject
    {
        /// <summary>
        /// The X position on the image
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The y position on the image
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// A reference to the astronomical object mapped to the coordinates on the image
        /// </summary>
        public AstronomicalObjectInfo AstroObject { get; set; }
    }

    public class PlateSolvedAndMarkedImage
    {
        /// <summary>
        /// A list of mapped object on the image including their x/y coordinates on the image
        /// </summary>
        public List<MappedAstroObject> MappedObjectsInImage { get; set; }

        /// <summary>
        /// Image with markings
        /// </summary>
        public SKBitmap MarkedImage { get; set; }

        /// <summary>
        /// CSV that contains all the objects with properties 
        /// </summary>
        public byte[] InfoCSV { get; set; }
    }
}
