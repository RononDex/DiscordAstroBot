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
            var token = File.ReadAllText(@"C:\Users\tinoh\Desktop\astroToken.txt");
            
            // Login into Astrometry
            var sessionID = DiscordAstroBot.Helpers.AstrometryHelper.LoginIntoAstrometry(token);
            var submissionID = "1791271";

            // Wait for completion (around 60s)
            //Thread.Sleep(60 * 1000);

            var result = DiscordAstroBot.Helpers.AstrometryHelper.GetSubmissionStatus(submissionID);

            if (result.State == AstrometrySubmissionState.JOB_FINISHED)
            {
                var jobId = result.JobID.Value;

                var calibData = AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());

                // Mark stuff in the image
                var img = new Bitmap("TestData/Astrometry/w76hvjz.png");

                DiscordAstroBot.Utilities.AdvancedPlateSolver.MarkObjectsOnImage(img, calibData);

                img.Save(@"C:\Users\tinoh\Desktop\test.jpg", ImageFormat.Jpeg);
            }
        }
    }
}
