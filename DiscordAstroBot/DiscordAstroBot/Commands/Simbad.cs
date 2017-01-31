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
                var info = SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["AstroObject"].Value);

                if (info == null)
                {
                    e.Channel.SendMessage(string.Format("Could not find any object matching your search \"{0}\" in the SIMBAD database", matchedMessage.Groups["AstroObject"].Value));
                    return;
                }

                var obj = Objects.AstronomicalObjectInfo.FromSimbadResult(info);
                e.Channel.SendMessage(string.Format("This is what I found in the SIMBAD database:\r\n" +
                                                    "```\r\n" +
                                                    "Main Identifier: {0}\r\n" +
                                                    "MainType: {1}\r\n" +
                                                    "Coordinates:\r\n{2}\r\n\r\n" +
                                                    "Radial velocity:\r\n{4}\r\n\r\n" +
                                                    "Parallax:\r\n{5}\r\n\r\n" +
                                                    "Proper motion:\r\n{6}\r\n\r\n" +
                                                    "SecondaryTypes:\r\n{3}\r\n\r\n" +
                                                    "Also known as: \r\n{{7}\r\n\r\n" +
                                                    "```\r\n",
                    obj.Name,
                    obj.ObjectType,
                    obj.Coordinates,
                    string.Join(", ", obj.SecondaryTypes.Select(x => x.Replace("\n", "").Replace("\r", ""))),
                    obj.RadialVelocity,
                    obj.Parallax,
                    obj.ProperMotion,
                    string.Join(", ", obj.AlsoKnownAs)));
            }
        }
    }
}
