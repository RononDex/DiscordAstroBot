using Discord;
using DiscordAstroBot.Helpers;
using DiscordAstroBot.Simbad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordAstroBot.Commands
{
    public class Simbad : Command
    {
        public override string CommandName { get { return "Simbad"; } }

        public override string[] CommandSynonyms
        {
            get
            {
                return new string[] {
                    @"what do you know about (?'AstroObject'.*\w)(\?)?"
                };
            }
        }

        public override void MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            if (matchedMessage.Groups["AstroObject"].Success)
            {
                e.Channel.SendMessage("Querying the SIMBAD database, please wait...");

                var info = SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["AstroObject"].Value);

                if (info == null)
                {
                    e.Channel.SendMessage(string.Format("Could not find any object matching your search \"{0}\" in the SIMBAD database", matchedMessage.Groups["AstroObject"].Value));
                    return;
                }

                var obj = Objects.AstronomicalObjectInfo.FromSimbadResult(info);
                e.Channel.SendMessage($"This is what I found in the SIMBAD database:\r\n" +
                                    $"```\r\n" +
                                    $"Main Identifier: {obj.Name}\r\n" +
                                    $"MainType: {obj.ObjectType}\r\n" +
                                    $"Coordinates:\r\n{obj.Coordinates}\r\n\r\n" +
                                    $"Distance: \r\n{string.Join("\r\n", obj.DistanceMeasurements)}\r\n\r\n" +
                                    $"Radial velocity:\r\n{obj.RadialVelocity}\r\n\r\n" +
                                    $"Parallax:\r\n{obj.Parallax}\r\n\r\n" +
                                    $"Proper motion:\r\n{obj.ProperMotion}\r\n\r\n" +
                                    $"```\r\n");

                e.Channel.SendMessage($"```\r\n" +
                                    $"SecondaryTypes:\r\n{string.Join(", ", obj.SecondaryTypes.Select(x => x.Replace("\n", "").Replace("\r", "")))}\r\n\r\n" +
                                    $"Also known as: \r\n{string.Join(", ", obj.AlsoKnownAs)}\r\n\r\n"+
                                    $"```\r\n");
            }
        }
    }
}
