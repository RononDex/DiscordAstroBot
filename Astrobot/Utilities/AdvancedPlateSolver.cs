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
    /// SIMBAD to get the objects and WCS coordinate transformations from astrophy to 
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
        public static Bitmap MarkObjectsOnImage(Bitmap plateSolvedImage,
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
                        "wcs.fit"
                    }));
            
            return plateSolvedImage;
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
            var csvFile = objects.Aggregate(string.Empty, (current, obj) => current + $"{obj.Coordinates.RA};{obj.Coordinates.DEC}\r\n");
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
            var memoryStream = AstrometryHelper.DownloadWCSFitsFile(plateSolveResult.JobID);

            // Save to file to disk
            memoryStream.WriteTo(File.Create(targetFile));
        }
    }
}
