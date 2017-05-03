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
                    @"enable all commands(?'EnableAllCommands'[^\s]+)",
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
                    List<CommandConfigServerCommand> commands = new List<CommandConfigServerCommand>();
                    if (server != null)
                        commands = server.Commands;
                    else
                    {
                        Config.CommandsConfig.CommandsConfigServer.Add(new CommandsConfigServer() { ServerID = e.Server.Id, Commands = commands });
                        Config.CommandsConfig.SaveConfig();
                    }
                    
                    if (commands.Count == 0)
                        e.Channel.SendMessage("There are currently no enabled commands :(");
                    else
                        e.Channel.SendMessage($"These commands are currently enabled:\r\n```\r\n{string.Join("\r\n", commands.Select(x => x.CommandName))}\r\n```");
                }

                // List enabled commands on this server
                if (matchedMessage.Groups["AvailableCommandList"].Success)
                {
                    var commands = DiscordAstroBot.Commands;
                    e.Channel.SendMessage($"The following commands are currently registered and can be used on this server:\r\n```\r\n{string.Join("\r\n", commands.Select(x => x.CommandName))}\r\n```");
                }

                // Enable a command
                if (matchedMessage.Groups["EnableCommandName"].Success)
                {
                    var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == e.Server.Id);
                    List<CommandConfigServerCommand> commands = new List<CommandConfigServerCommand>();
                    if (server != null)
                        commands = server.Commands;
                    else
                    {
                        Config.CommandsConfig.CommandsConfigServer.Add(new CommandsConfigServer() { ServerID = e.Server.Id, Commands = commands });
                        Config.CommandsConfig.SaveConfig();
                    }

                    if (commands.Any(x => x.CommandName.ToLower() == matchedMessage.Groups["EnableCommandName"].Value.ToLower() && x.Enabled))
                        e.Channel.SendMessage("Command is already enabled");
                    else
                    {
                        if (!commands.Any(x => x.CommandName.ToLower() == matchedMessage.Groups["EnableCommandName"].Value.ToLower()))
                            commands.Add(new CommandConfigServerCommand() { CommandName = matchedMessage.Groups["EnableCommandName"].Value.ToLower(), Enabled = true });
                        else
                            commands.First(x => x.CommandName.ToLower() == matchedMessage.Groups["EnableCommandName"].Value.ToLower()).Enabled = true;

                        Config.CommandsConfig.SaveConfig();
                        e.Channel.SendMessage($"Command {matchedMessage.Groups["EnableCommandName"].Value} is now enabled");
                    }
                }

                // Disables a command
                if (matchedMessage.Groups["DisableCommandName"].Success)
                {
                    if (matchedMessage.Groups["DisableCommandName"].Value.ToLower() == "admincommands")
                    {
                        e.Channel.SendMessage("You can't disable the AdminCommands, that would render me on this server unmanagable!");
                        return true;
                    }

                    var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == e.Server.Id);
                    List<CommandConfigServerCommand> commands = new List<CommandConfigServerCommand>();
                    if (server != null)
                        commands = server.Commands;
                    else
                    {
                        Config.CommandsConfig.CommandsConfigServer.Add(new CommandsConfigServer() { ServerID = e.Server.Id, Commands = commands });
                        Config.CommandsConfig.SaveConfig();
                    }

                    if (commands.Any(x => x.CommandName.ToLower() == matchedMessage.Groups["DisableCommandName"].Value.ToLower() && !x.Enabled))
                        e.Channel.SendMessage("Command is already disabled");
                    else
                    {
                        if (!commands.Any(x => x.CommandName.ToLower() == matchedMessage.Groups["DisableCommandName"].Value.ToLower()))
                            commands.Add(new CommandConfigServerCommand() { CommandName = matchedMessage.Groups["DisableCommandName"].Value.ToLower(), Enabled = false });
                        else
                            commands.First(x => x.CommandName.ToLower() == matchedMessage.Groups["DisableCommandName"].Value.ToLower()).Enabled = false;

                        Config.CommandsConfig.SaveConfig();
                        e.Channel.SendMessage($"Command {matchedMessage.Groups["DisableCommandName"].Value} is now disabled");
                    }
                }

                // Enable all the commands
                if (matchedMessage.Groups["EnableAllCommands"].Success)
                {
                    var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == e.Server.Id);
                    List<CommandConfigServerCommand> commands = new List<CommandConfigServerCommand>();
                    if (server != null)
                        commands = server.Commands;
                    else
                    {
                        Config.CommandsConfig.CommandsConfigServer.Add(new CommandsConfigServer() { ServerID = e.Server.Id, Commands = commands });
                        Config.CommandsConfig.SaveConfig();
                    }

                    foreach (var command in DiscordAstroBot.Commands)
                    {
                        if (!commands.Any(x => x.CommandName.ToLower() == command.CommandName.ToLower()))
                            commands.Add(new CommandConfigServerCommand() { CommandName = command.CommandName.ToLower(), Enabled = true });
                        else
                            commands.First(x => x.CommandName.ToLower() == command.CommandName.ToLower()).Enabled = true;

                        Config.CommandsConfig.SaveConfig();
                        e.Channel.SendMessage($"All the commands are now enabled on this server!");
                    }
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
