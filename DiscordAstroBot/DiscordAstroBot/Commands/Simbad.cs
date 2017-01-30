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

                if ( info == null )
                {
                    e.Channel.SendMessage(string.Format("Could not find any object matching your search \"{0}\" in the SIMBAD database", matchedMessage.Groups["AstroObject"].Value));
                    return;
                }

                var obj = Objects.AstronomicalObjectInfo.FromSimbadResult(info);
                e.Channel.SendMessage(string.Format("This is what I found in the SIMBAD database:\r\n```\r\nMain Identifier: {0}\r\nMainType: {1}\r\nCoordinates:\r\n{2}\r\n\r\nRadial velocity:\r\n{4}\r\nParallax:\r\n\r\n{5}\r\n\r\nProper motion:\r\n{6}\r\n\r\nSecondaryTypes:\r\n{3}\r\n\r\n```\r\n",
                    obj.Name,
                    obj.ObjectType,
                    obj.Coordinates,
                    string.Join(", ", obj.SecondaryTypes.Select( x => x.Replace("\n", "").Replace("\r", ""))),
                    obj.RadialVelocity,
                    obj.Parallax,
                    obj.ProperMotion));
            }
        }
    }
}
