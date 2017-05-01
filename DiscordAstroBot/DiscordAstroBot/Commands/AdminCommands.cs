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
    /// Controls at who the bot is mad at
    /// </summary>
    public class AdminCommands : Command
    {
        public override string[] CommandSynonyms
        {
            get { return new[] { "^|(go|be|) mad at (?'MadUser'.*)" }; }
        }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            // Make sure user is admin
            if (e.User.Roles.Any(x => x.Permissions.Administrator))
            {
                if (matchedMessage.Groups["MadUser"].Success)
                {
                    
                }
                return true;
            }
            else
            {
                e.Channel.SendMessage("UNAUTHORIZED ACCESS DETECTED!\r\nSThis command is only for admins!");
                return true;
            }
        }
    }
}
