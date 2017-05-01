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
        public override string CommandName
        {
            get
            {
                return "AdminCommands";
            }
        }

        public override string[] CommandSynonyms
        {
            get
            {
                return new[]
                {
                    @"toggle mad mode for (?'MadUser'.*)",
                    @"who are you mad at(?'MadUserList')(\?)?",
                    @"(what|which) commands are enabled(?'EnabledCommandsList')(\?)?"
                };
            }
        }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            // Make sure user is admin
            if (e.User.Roles.Any(x => x.Permissions.Administrator))
            {
                // Toggle mad user
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

                // List mad users
                if (matchedMessage.Groups["MadUserList"].Success)
                {
                    var users = Config.MadUsers.Users.Where(x => x.Server == e.Server.Id.ToString()).Select(y => e.Server.Users.FirstOrDefault(x => x.Id.ToString() == y.User).Name);
                    e.Channel.SendMessage($"I am currently mad at these fellow users:\r\n```\r\n{string.Join("\r\n", users)}\r\n```");
                }

                // List enabled commands on this server
                if (matchedMessage.Groups["EnabledCommandsList"].Success)
                {

                }
                return true;
            }
            else
            {
                e.Channel.SendMessage("UNAUTHORIZED ACCESS DETECTED!\r\nBut seriously, this command is only for admins (and you are not one of them, so...)!");
                return true;
            }
        }
    }
}
