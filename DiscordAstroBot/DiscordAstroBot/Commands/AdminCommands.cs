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
            get { return new[] { "toggle mad mode for (?'MadUser'.*)" }; }
        }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            // Make sure user is admin
            if (e.User.Roles.Any(x => x.Permissions.Administrator))
            {
                if (matchedMessage.Groups["MadUser"].Success)
                {
                    var user = matchedMessage.Groups["MadUser"].Value;
                    var resolvedUser = Utilities.DiscordUtilities.ResolveUser(e.Server, user);

                    if (resolvedUser == null)
                    {
                        e.Channel.SendMessage("Could not find any user with that name!");
                        return true;
                    }

                    var entry = Config.MadUsers.Users.FirstOrDefault(x => x.Server == e.Server.Id.ToString() && x.User == resolvedUser.Id.ToString());

                    // Stop being mad at the user
                    if (entry != null)
                    {
                        Config.MadUsers.Users.Remove(entry);
                        e.Channel.SendMessage($"No longer mad at {resolvedUser.Name}");
                    }
                    else
                    {
                        entry = new Objects.MadUser();
                        entry.Server = Convert.ToString(e.Server.Id);
                        entry.User = Convert.ToString(resolvedUser.Id);
                        Config.MadUsers.Users.Add(entry);
                        e.Channel.SendMessage($"Copy that! I am now mad at {resolvedUser.Name}");
                    }

                    // Save config
                    Config.MadUsers.SaveConfig();
                }
                return true;
            }
            else
            {
                e.Channel.SendMessage("UNAUTHORIZED ACCESS DETECTED!\r\nThis command is only for admins!");
                return true;
            }
        }
    }
}
