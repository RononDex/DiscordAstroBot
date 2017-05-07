using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using DiscordAstroBot.Objects.Config;

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
                    @"(what|which) commands are enabled(?'EnabledCommandsList')( on this server)?(\?)?",
                    @"(what|which) commands are (available|registered)(?'AvailableCommandList')( on this server)?(\?)?",
                    @"enable all commands(?'EnableAllCommands')",
                    @"enable( command)? (?'EnableCommandName'[^\s]+)",
                    @"disable( command)? (?'DisableCommandName'[^\s]+)",
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
                        entry = new MadUser();
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
                    var users = Config.MadUsers.Users.Where(x => x.Server == e.Server.Id.ToString()).Select(y => e.Server.Users.FirstOrDefault(x => x.Id.ToString() == y.User).Name).ToList();
                    if (users.Count == 0)
                        e.Channel.SendMessage("I am currently not mad at any users");
                    else
                        e.Channel.SendMessage($"I am currently mad at these users:\r\n```\r\n{string.Join("\r\n", users)}\r\n```");
                }

                // List enabled commands on this server
                if (matchedMessage.Groups["EnabledCommandsList"].Success)
                {
                    var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == e.Server.Id);
                    if (server == null)
                    {
                        server = Utilities.CommandsUtility.SetupServerConfig(e.Server);
                    }

                    var commands = server.Commands;
                    e.Channel.SendMessage($"The following commands are currently enabled on this server:\r\n```\r\n{string.Join("\r\n", commands.Select(x => x.CommandName))}\r\n```");
                }

                // List enabled commands on this server
                if (matchedMessage.Groups["AvailableCommandList"].Success)
                {
                    var commands = Utilities.CommandsUtility.GetAllRegisteredCommands();
                    e.Channel.SendMessage($"The following commands are currently registered and can be used on this server:\r\n```\r\n{string.Join("\r\n", commands.Select(x => x.CommandName))}\r\n```");
                }

                // Enable a command
                if (matchedMessage.Groups["EnableCommandName"].Success)
                {
                    var resolvedCmd = Utilities.CommandsUtility.ResolveCommand(matchedMessage.Groups["EnableCommandName"].Value);
                    if (resolvedCmd != null)
                    {
                        Utilities.CommandsUtility.EnableCommand(e.Server, resolvedCmd);
                        e.Channel.SendMessage($"Command {resolvedCmd.CommandName} is now enabled");
                    }
                    else
                    {
                        e.Channel.SendMessage($"Could not find any command called {matchedMessage.Groups["EnableCommandName"].Value}");
                    }
                }

                // Disables a command
                if (matchedMessage.Groups["DisableCommandName"].Success)
                {
                    var resolvedCmd = Utilities.CommandsUtility.ResolveCommand(matchedMessage.Groups["DisableCommandName"].Value);

                    // AdminCommands may not be disabled
                    if (resolvedCmd.CommandName.ToLower() == "admincommands")
                    {
                        e.Channel.SendMessage($"AdminCommands can not be disabled, or else how are you going to configure me on your server?");
                        return true;
                    }

                    if (resolvedCmd != null)
                    {
                        Utilities.CommandsUtility.DisableCommand(e.Server, resolvedCmd);
                        e.Channel.SendMessage($"Command {resolvedCmd.CommandName} is now disabled");
                    }
                    else
                    {
                        e.Channel.SendMessage($"Could not find any command called {matchedMessage.Groups["DisableCommandName"].Value}");
                    }
                }

                // Enable all the commands
                if (matchedMessage.Groups["EnableAllCommands"].Success)
                {
                    Utilities.CommandsUtility.EnableAllCommands(e.Server);
                    e.Channel.SendMessage("All Commands are now enabled!");
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
