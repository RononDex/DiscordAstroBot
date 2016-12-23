﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// The Sesame command is used to query the Sesame service, which is basically an object name resolver
    /// and gets all the data it can find from simbad, Vizier and Aladin
    /// </summary>
    public class Sesame : Command
    {
        public override string CommandName { get { return "Sesame"; } }

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
            var resolvedObject = Helpers.SesameHelper.ResolveWithSesame(matchedMessage.Groups["AstroObject"].Value);

            e.Channel.SendMessage(string.Format("```\r\nName: {1}\r\nJPOS2000: {2}\r\nDEC (deg): {3}\r\nRA (deg): {4}\r\nAliases: \r\n{0}\r\n```", 
                string.Join(", ", resolvedObject.Aliases), 
                resolvedObject.Name, 
                resolvedObject.PositionJ2000, 
                resolvedObject.DECDec, 
                resolvedObject.RADec));
        }
    }
}
