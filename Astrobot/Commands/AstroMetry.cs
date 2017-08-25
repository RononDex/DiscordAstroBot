using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Configuration;
using System.Threading;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    public class AstroMetry : Command
    {
        public override string[] CommandSynonyms => new string[]
        {
            @"(can (you )?)?(please )?(analy(s|z)e|plate(-|\s)?solve) this (image|photo)(\?)?",
            @"what can you (find|figure) out (about|on|from|for) this (image|photo)(\?)?",
            @"what (space )?(objects|DSO) (are|do you know) in this (image|photo)(\?)?"
        };

        public override string CommandName => "Astrometry";

        public override string Description
            =>
                "This is an advanced plate solving / image analysis command. Usage (attach an image to your message): \r\n```    @Astro Bot analyse this image```";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            try
            {
                // Check if there is an image attached
                if (recievedMessage.Attachments.Count == 0)
                {
                    await recievedMessage.Channel.SendMessageAsync("No file attached, please attach a file");
                    return true;
                }

                // Login into Astrometry
                await recievedMessage.Channel.SendMessageAsync("Submitting your image to astrometry for analysis and plate-solving...\r\n(Depending on the image, this might take a few minutes, be patient...)");
                string sessionID = Helpers.AstrometryHelper.LoginIntoAstrometry(File.ReadAllText(ConfigurationManager.AppSettings["AstrometryTokenFilePath"]));

                string submissionID = Helpers.AstrometryHelper.UploadFile(recievedMessage.Attachments.First().Url, recievedMessage.Attachments.First().Filename, sessionID);
                await recievedMessage.Channel.SendMessageAsync($"Submission successfull: **{submissionID}**. Awaiting results...");

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
                    await recievedMessage.Channel.SendMessageAsync(string.Format("__**WARNING:**__ Astrometry could not finish the image analysis within {0} minutes for your submission **{1}**. Please check the result yourself on the provided submission link\r\n http://nova.astrometry.net/status/{1}", maxWait / 1000 / 60, submissionID));
                    return true;
                }

                await recievedMessage.Channel.SendMessageAsync($"Image analysis for submission **{submissionID}** successfull. Here are the results:");
                var calibrationData = Helpers.AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());
                var objectsInImage = string.Join(", ", calibrationData.ObjectsInfField);
                var tags = string.Join(", ", calibrationData.Tags);
                await recievedMessage.Channel.SendMessageAsync($"```\r\nRA: {calibrationData.CalibrationData.RA}\r\nDEC: {calibrationData.CalibrationData.DEC}\r\nOrientation: up is {calibrationData.CalibrationData.Orientation} deg\r\nRadius: {calibrationData.CalibrationData.Radius} deg\r\nPixelScale: {calibrationData.CalibrationData.PixScale} arcsec/pixel\r\nObjectsInImage: {objectsInImage}\r\nTags: {tags}\r\n```");

                await recievedMessage.Channel.SendFileAsync(Helpers.AstrometryHelper.DownlaodAnnotatedImage(jobId.ToString()), $"annoated_{calibrationData.FileName}");
                await recievedMessage.Channel.SendMessageAsync($"Link to astrometry job result: http://nova.astrometry.net/status/{submissionID}");

                return true;
            }
            catch (Exception ex)
            {
                await recievedMessage.Channel.SendMessageAsync($"Oh noes! Something you did caused me to crash: {ex.Message}");
                Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", recievedMessage.Content, ex.Message);
                return true;
            }
        }
    }
}
