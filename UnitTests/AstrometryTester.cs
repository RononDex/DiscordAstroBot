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
            var submissionID = DiscordAstroBot.Helpers.AstrometryHelper.UploadFile("http://nova.astrometry.net/image/3882132", "Astro Bot Testing", sessionID);

            // Wait for completion (around 60s)
            Thread.Sleep(60 * 1000);

            var result = DiscordAstroBot.Helpers.AstrometryHelper.GetSubmissionStatus(submissionID);

            if (result.State == AstrometrySubmissionState.JOB_FINISHED)
            {
                var jobId = result.JobID.Value;

                var calibData = AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());

                // Mark stuff in the image
                var img = new Bitmap("TestData/Astrometry/3882132.jpg");

                DiscordAstroBot.Utilities.AdvancedPlateSolver.MarkObjectsOnImage(img, calibData);                

                var objInImage = DiscordAstroBot.Mappers.Simbad.SimbadQuery.QueryAround(new RADECCoords()
                {
                    RA = calibData.CalibrationData.RA,
                    DEC = calibData.CalibrationData.DEC
                }, calibData.CalibrationData.Radius * 1.5f);

                

                var angle = 360 - calibData.CalibrationData.Orientation;
                var centerPoint = new PointF(img.Width / 2f, img.Height / 2f);

                foreach (var item in objInImage.Where(x => x.Sections.Count > 0))
                {
                    var degPerPx = calibData.CalibrationData.PixScale / 60f / 60f;
                    var info = AstronomicalObjectInfo.FromSimbadResult(item);
                    if (info.Coordinates == null) continue;

                    var raDecPoint =
                        RotatePoint(
                                    new PointF(((calibData.CalibrationData.RA - info.Coordinates.RA) / (degPerPx*1.2f)) + centerPoint.X,
                                               ((calibData.CalibrationData.DEC - info.Coordinates.DEC  ) / (degPerPx)) + centerPoint.Y),
                                    centerPoint,
                                    angle);

                    DiscordAstroBot.Utilities.ImageUtility.AddCrossMarker(img, Convert.ToInt32(centerPoint.X - (2*( raDecPoint.X - centerPoint.X))), raDecPoint.Y);
                    DiscordAstroBot.Utilities.ImageUtility.AddLabel(img, Convert.ToInt32(centerPoint.X - (2 * (raDecPoint.X - centerPoint.X))), raDecPoint.Y, info.Name);
                }

                img.Save(@"C:\Users\het\Desktop\test.jpg", ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The center point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        static Point RotatePoint(PointF pointToRotate, PointF centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
