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
        public override string[] CommandSynonyms
        {
            get
            {
                return new string[]
                {
                    @"(can (you )?)?(please )?(analy(s|z)e|plate(-|\s)?solve) this (image|photo)(\?)?",
                    @"what can you (find|figure) out (about|on|from|for) this (image|photo)(\?)?",
                    @"what (space )?(objects|DSO) (are|do you know) in this (image|photo)(\?)?"
                };
            }
        }

        public override string CommandName { get { return "Astrometry"; } }

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

                await recievedMessage.Channel.SendMessageAsync(string.Format("Image analysis for submission **{0}** successfull. Here are the results:", submissionID));
                var calibrationData = Helpers.AstrometryHelper.GetCalibrationFromFinishedJob(jobId.ToString());
                var objectsInImage = string.Join(", ", calibrationData.ObjectsInfField);
                var tags = string.Join(", ", calibrationData.Tags);
                await recievedMessage.Channel.SendMessageAsync(string.Format("```\r\nRA: {0}\r\nDEC: {1}\r\nOrientation: up is {2} deg\r\nRadius: {3} deg\r\nPixelScale: {4} arcsec/pixel\r\nObjectsInImage: {5}\r\nTags: {6}\r\n```",
                    calibrationData.CalibrationData.RA,
                    calibrationData.CalibrationData.DEC,
                    calibrationData.CalibrationData.Orientation,
                    calibrationData.CalibrationData.Radius,
                    calibrationData.CalibrationData.PixScale,
                    objectsInImage,
                    tags));

                await recievedMessage.Channel.SendFileAsync(Helpers.AstrometryHelper.DownlaodAnnotatedImage(jobId.ToString()), string.Format("annoated_{0}", calibrationData.FileName));
                await recievedMessage.Channel.SendMessageAsync(string.Format("Link to astrometry job result: http://nova.astrometry.net/status/{0}", submissionID));

                return true;
            }
            catch (Exception ex)
            {
                await recievedMessage.Channel.SendMessageAsync(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", recievedMessage.Content, ex.Message);
                return true;
            }
        }
    }
}
