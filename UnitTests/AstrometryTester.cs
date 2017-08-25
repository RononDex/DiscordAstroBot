using System;
using System.IO;
using System.Threading;
using DiscordAstroBot.Helpers;
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
            var jobID = DiscordAstroBot.Helpers.AstrometryHelper.UploadFile("http://nova.astrometry.net/image/3882132", "test", sessionID);

            // Wait for completion (around 30s)
            Thread.Sleep(30 * 1000);

            var result = DiscordAstroBot.Helpers.AstrometryHelper.GetSubmissionStatus(jobID);

            if (result.State == AstrometrySubmissionState.JOB_FINISHED)
            {
                var calibData = AstrometryHelper.GetCalibrationFromFinishedJob(jobID);
            }
        }
    }
}
