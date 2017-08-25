using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using DiscordAstroBot.Helpers;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Command that allows access to the DSS database, and to query images using the name resolver from SIMBAD
    /// </summary>
    public class DSSCommand : Command
    {
        public override string CommandName => "DSS";

        public override string[] CommandSynonyms => new string[] { @"(how|what) does (?'ObjectName'.+?(?= look)) look like(\?)?" };

        public override string Description => "Query DSS database for how some deep space object looks like. Usage:\r\n```    @Astro Bot How does M63 look like```";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            try
            {
                if (matchedMessage.Groups["ObjectName"].Success)
                {
                    // Search for object in SIMBAD database
                    var info =
                        Mappers.Simbad.SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["ObjectName"].Value);

                    if (info == null)
                    {
                        await recievedMessage.Channel.SendMessageAsync(
                            $"Could not find any obejct with the name \"{matchedMessage.Groups["ObjectName"].Value}\" in the SIMBAD database");
                        return true;
                    }

                    // Check if RA and DEC coordinates are known
                    var parsedInfo = Objects.Simbad.AstronomicalObjectInfo.FromSimbadResult(info);
                    if (string.IsNullOrEmpty(parsedInfo.Coordinates.DEC) ||
                        string.IsNullOrEmpty(parsedInfo.Coordinates.RA))
                    {
                        await recievedMessage.Channel.SendMessageAsync(
                            $"The object was found in the SIMBAD database, but no RA,DEC coordinates are known.");
                        return true;
                    }

                    await recievedMessage.Channel.SendMessageAsync("Querying the DSS server for the image. One moment...");

                    var image = DSSImageHelper.GetImage(parsedInfo.Coordinates.RA, parsedInfo.Coordinates.DEC);
                    var stream = new MemoryStream(image);
                    await recievedMessage.Channel.SendMessageAsync(
                        $"Here is the result for RA: {parsedInfo.Coordinates.RA}, DEC: {parsedInfo.Coordinates.DEC}, Radius: 60 arcminutes");
                    await recievedMessage.Channel.SendFileAsync(stream, $"DSS2.jpg");

                    return true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return false;
        }
    }
}
