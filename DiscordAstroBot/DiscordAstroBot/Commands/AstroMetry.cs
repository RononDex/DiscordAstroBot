using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Configuration;

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
            e.Message.Channel.SendMessage(string.Format("Submission successfull: {0}. Awaiting results...", submissionID));
            e.Message.Channel.SendMessage(string.Format("See status here: http://nova.astrometry.net/status/{0}", submissionID));
        }
    }
}
