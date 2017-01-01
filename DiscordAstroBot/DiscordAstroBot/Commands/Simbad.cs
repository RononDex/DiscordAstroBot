﻿using Discord;
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
                e.Channel.SendMessage(string.Format("This is what I found in the SIMBAD databse:\r\n```\r\nMain Identifier: {0}\r\n```\r\n",
                    obj.Name));
            }
        }
    }
}
