using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using DiscordAstroBot.Helpers;

namespace DiscordAstroBot.Commands
{
    public class DSSCommand : Command
    {
        public override string CommandName => "DSS";

        public override string[] CommandSynonyms =>
        new string[] { @"(how|what) does (?'ObjectName'\w*) look like(\?)?" };

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            if (matchedMessage.Groups["ObjectName"].Success)
            {
                // Search for object in SIMBAD database
                var info = Mappers.Simbad.SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["ObjectName"].Value);

                if (info == null)
                {
                    e.Channel.SendMessage($"Could not find any obejct with the name \"{matchedMessage.Groups["ObjectName"].Value}\" in the SIMBAD database");
                    return true;
                }

                // Check if RA and DEC coordinates are known
                var parsedInfo = Objects.Simbad.AstronomicalObjectInfo.FromSimbadResult(info);
                if (string.IsNullOrEmpty(parsedInfo.Coordinates.DEC) || string.IsNullOrEmpty(parsedInfo.Coordinates.RA))
                {
                    e.Channel.SendMessage($"The object was found in the SIMBAD database, but no RA,DEC coordinates are known.");
                    return true;
                }

                e.Channel.SendMessage("Give me a sec... getting the image from the DSS server");

                var image = DSSImageHelper.GetImage(parsedInfo.Coordinates.RA, parsedInfo.Coordinates.DEC);
                var stream = new MemoryStream(image);
                e.Channel.SendMessage($"Here is the result for RA: {parsedInfo.Coordinates.RA}, DEC: {parsedInfo.Coordinates.DEC}, Radius: 60 arcminutes");
                e.Channel.SendFile($"DSS2", stream);

                return true;
            }
            return false;
        }
    }
}
