using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using DiscordAstroBot.Helpers;
using DiscordAstroBot.Objects.Simbad;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace UnitTests
{
    [TestClass]
    public class AstrometryTester
    { 
        [TestMethod]
        public void PlateSolverTest()
        {
            var token = File.ReadAllText(@"\\10.0.0.2\Documents\Tino\AstrometryToken.txt");
            
            // Login into Astrometry
            var sessionID = DiscordAstroBot.Helpers.AstrometryHelper.LoginIntoAstrometry(token);
            //var submissionID = "1844860";
            var submissionID = AstrometryHelper.UploadFile("https://cdn.discordapp.com/attachments/254942175387713537/403469836300320768/IMG_0754_Large.jpg", "test.jpg", sessionID);

            // Wait for completion (around 60s)
            //Thread.Sleep(60 * 1000);

            var result = DiscordAstroBot.Helpers.AstrometryHelper.GetSubmissionStatus(submissionID);

            if (result.State == AstrometrySubmissionState.JOB_FINISHED)
            {
                var jobId = result.JobID.Value;

                var calibData = AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());
                using (var stream = new SKManagedStream(new FileStream(@"E:\Astro\2017-11-21_NGC891\edit1.jpg", FileMode.Open)))
                using (var bitmap = SKBitmap.Decode(stream))
                {
                    var res = DiscordAstroBot.Utilities.AdvancedPlateSolver.MarkObjectsOnImage(bitmap, calibData);
                    File.WriteAllBytes(@"C:\Users\tinoh\Desktop\test.csv", res.InfoCSV);

                    using (Stream s = File.OpenWrite(@"C:\Users\tinoh\Desktop\test.jpg"))
                    {
                        var d = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Jpeg, 80);
                        d.SaveTo(s);
                    }

                }
            }
        }
    }
}
