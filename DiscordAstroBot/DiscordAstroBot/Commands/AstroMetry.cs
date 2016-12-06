using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Configuration;
using System.Threading;

namespace DiscordAstroBot.Commands
{
    public class AstroMetry : Command
    {
        public override string[] CommandSynonyms
        {
            get
            {
                return new string[]
                {
                    "analyse this image",
                    "analyze this image",
                    "can you analyze this image",
                    "can you analyse this image",
                    "platesolve this image",
                    "can you platesolve this image",
                    "plate solve this image",
                    "can you plate solve this image",
                };
            }
        }

        public override string CommandName { get { return "Astrometry"; } }

        public override void MessageRecieved(string message, MessageEventArgs e)
        {
            // Check if there is an image attached
            if (e.Message.Attachments.Length == 0)
            {
                e.Channel.SendMessage("No file attached, please attach a file");
                return;
            }

            // Login into Astrometry
            e.Message.Channel.SendMessage("Submitting your image to astrometry for analysis and plate-solving...");
            string sessionID = Helpers.AstrometryHelper.LoginIntoAstrometry(File.ReadAllText(ConfigurationManager.AppSettings["AstrometryTokenFilePath"]));

            string submissionID = Helpers.AstrometryHelper.UploadFile(e.Message.Attachments[0].Url, e.Message.Attachments[0].Filename, sessionID);
            e.Message.Channel.SendMessage(string.Format("Submission successfull: **{0}**. Awaiting results...", submissionID));
            //e.Message.Channel.SendMessage(string.Format("See status here: http://nova.astrometry.net/status/{0}", submissionID));

            var waitDelta = 5000;
            var maxWait = 300 * 1000;
            var curWait = 0;
            var jobId = 0;
            var finished = false;
            while (curWait <= maxWait)
            {
                var status = Helpers.AstrometryHelper.GetSubmissionStatus(submissionID);

                if (status.JobID != null)
                    jobId = status.JobID.Value;

                if (status.State == Helpers.AstrometrySubmissionState.JOB_FINISHED)
                {
                    finished = true;
                    break;
                }

                curWait += waitDelta;
                Thread.Sleep(waitDelta);
            }

            if (!finished)
            {
                e.Channel.SendMessage(string.Format("__**WARNING:**__ Astrometry could not finish the image analysis within {0} minutes for your submission **{1}**. Please check the result yourself on the provided submission link", maxWait / 1000 / 60, submissionID));
                return;
            }

            e.Channel.SendMessage(string.Format("Image analysis for submission **{0}** successfull. Here are the results:", submissionID));
            var calibrationData = Helpers.AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());
            var objectsInImage = string.Join(", ", calibrationData.ObjectsInfField);
            var tags = string.Join(", ", calibrationData.Tags);
            e.Channel.SendMessage(string.Format("```\r\nRA: {0}\r\nDEC: {1}\r\nOrientation: up is {2} deg\r\nRadius: {3} deg\r\nPixelScale: {4} arcsec/pixel\r\nObjectsInImage: {5}\r\nTags: {6}\r\n```", 
                calibrationData.CalibrationData.RA, 
                calibrationData.CalibrationData.DEC, 
                calibrationData.CalibrationData.Orientation, 
                calibrationData.CalibrationData.Radius, 
                calibrationData.CalibrationData.PixScale, 
                objectsInImage,
                tags));

            e.Channel.SendFile(string.Format("annoated_{0}", calibrationData.FileName),Helpers.AstrometryHelper.DownlaodAnnotatedImage(jobId.ToString()));
            e.Channel.SendMessage(string.Format("Link to astrometry job result: http://nova.astrometry.net/user_images/{0}", submissionID));
        }
    }
}
