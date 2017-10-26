using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using DiscordAstroBot.Helpers;
using DiscordAstroBot.Objects.Simbad;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class AstrometryTester
    {

        [TestMethod]
        public void PlateSolverTest()
        {
            var token = File.ReadAllText(@"C:\Users\het\Desktop\astroToken.txt");

            // Login into Astrometry
            var sessionID = DiscordAstroBot.Helpers.AstrometryHelper.LoginIntoAstrometry(token);
            var submissionID = DiscordAstroBot.Helpers.AstrometryHelper.UploadFile("http://nova.astrometry.net/image/3882132", "test", sessionID);

            // Wait for completion (around 60s)
            Thread.Sleep(60 * 1000);

            var result = DiscordAstroBot.Helpers.AstrometryHelper.GetSubmissionStatus(submissionID);

            if (result.State == AstrometrySubmissionState.JOB_FINISHED)
            {
                var jobId = result.JobID.Value;
                var calibData = AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());

                var objInImage = DiscordAstroBot.Mappers.Simbad.SimbadQuery.QueryAround(new RADECCoords()
                {
                    RA = calibData.CalibrationData.RA,
                    DEC = calibData.CalibrationData.DEC
                }, calibData.CalibrationData.Radius);

                // Mark stuff in the image
                var img = new Bitmap("TestData/Astrometry/3882132.jpg");

                foreach (var item in objInImage.Where(x => x.Sections.Count > 0))
                {
                    var degPerPx = calibData.CalibrationData.PixScale / 60d / 60d;
                    var info = AstronomicalObjectInfo.FromSimbadResult(item);
                    if (info.Coordinates == null) continue;
                    var raPx = (calibData.CalibrationData.RA - info.Coordinates.RA) / degPerPx;
                    var decPx = (calibData.CalibrationData.DEC - info.Coordinates.DEC) / degPerPx;
                    var angle = Math.PI * (360-calibData.CalibrationData.Orientation / 180.0 );

                    // Rotate coordinates to match the image rotation
                    var raPxRot = (raPx * Math.Cos(angle)) -
                                  decPx * Math.Sin(angle);
                    var decPxRot = (decPx * Math.Cos(angle)) -
                                   raPx * Math.Sin(angle);

                    DiscordAstroBot.Utilities.ImageUtility.AddCrossMarker(img, Convert.ToInt32((img.Width / 2) - raPxRot), Convert.ToInt32((img.Height / 2) - decPxRot));
                    DiscordAstroBot.Utilities.ImageUtility.AddLabel(img, Convert.ToInt32((img.Width / 2) - raPxRot), Convert.ToInt32((img.Height / 2) - decPxRot), info.Name);
                }

                img.Save(@"C:\Users\het\Desktop\test.jpg", ImageFormat.Jpeg);
            }
        }
    }
}
