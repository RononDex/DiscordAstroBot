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
        public static PlateSolvedAndMarkedImage MarkObjectsOnImage(Bitmap plateSolvedImage,
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

            return new PlateSolvedAndMarkedImage() { MappedObjectsInImage = transformedObjects, MarkedImage = plateSolvedImage };
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
                calibrationData.Radius * 1.8f);

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
            var fileText = File.OpenText(file);
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

            return res;
        }

        /// <summary>
        /// Marks the given objects on the image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="objects"></param>
        private static void MarkObjectsOnImage(Bitmap image, List<MappedAstroObject> objects, AstrometrySubmissionCalibrationData calibrationData)
        {
            foreach (var obj in objects.Where(x => x.AstroObject != null))
            {
                // If the object has known dimensions, add a ellipse around its
                if (obj.AstroObject.AngularDimension != null)
                {
                    var radius = new[] { obj.AstroObject.AngularDimension.XSize / (calibrationData.PixScale / 60), obj.AstroObject.AngularDimension.YSize / (calibrationData.PixScale / 60) }.Max();

                    ImageUtility.AddEllipse(
                        image,
                        obj.X,
                        obj.Y,
                        radius,
                        radius,
                        +calibrationData.Orientation - obj.AstroObject.AngularDimension.Rotation,
                        image.Width / 1500);                    

                    ImageUtility.AddLabel(image, Convert.ToInt32(obj.X + radius - 0.6 * radius), Convert.ToInt32(obj.Y + radius - 0.6 * radius), image.Width / 225, true, obj.AstroObject.Name);
                }
                // Per default mark objects with a crosshair + label
                else
                {
                    ImageUtility.AddCrossMarker(image, Convert.ToInt32(obj.X), Convert.ToInt32(obj.Y));
                    ImageUtility.AddLabel(image, Convert.ToInt32(obj.X + 0.0015 * image.Width), Convert.ToInt32(obj.Y + 0.0015 * image.Height), image.Width / 500, false, obj.AstroObject.Name);
                }
            }
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
        public Bitmap MarkedImage { get; set; }
    }
}
