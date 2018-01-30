using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using DiscordAstroBot.Objects.Config;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Controls at who the bot is mad at
    /// </summary>
    public class AdminCommands : Command
    {
        public override string CommandName => "AdminCommands";

        public override CommandSynonym[] CommandSynonyms => new[]
        {
            new CommandSynonym() { Synonym = @"toggle mad mode for (?'MadUser'.*)" },
            new CommandSynonym() { Synonym = @"who are you mad at(?'MadUserList')(\?)?" },
            new CommandSynonym() { Synonym = @"(what|which) commands are enabled(?'EnabledCommandsList')( on this server)?(\?)?" },
            new CommandSynonym() { Synonym = @"(what|which) commands are (available|registered)(?'AvailableCommandList')( on this server)?(\?)?" },
            new CommandSynonym() { Synonym = @"enable all commands(?'EnableAllCommands')" },
            new CommandSynonym() { Synonym = @"enable command (?'EnableCommandName'[^\s]+)" },
            new CommandSynonym() { Synonym = @"disable command (?'DisableCommandName'[^\s]+)" },
            new CommandSynonym() { Synonym = @"(?'ListSettings'show config)" },
            new CommandSynonym() { Synonym = @"setconfig (?'SetConfigKey'[^\s]*) (?'SetConfigValue'[^.*]*)" }
        };

        public override string Description => "Just some admin stuff";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            // Make sure user is admin
            if (((SocketGuildUser)recievedMessage.Author).Roles.Any(x => x.Permissions.Administrator))
            {
                // Toggle mad user
                if (matchedMessage.Groups["MadUser"].Success)
                {
                    var user = matchedMessage.Groups["MadUser"].Value;
                    var resolvedUser = Utilities.DiscordUtility.ResolveUser(((SocketGuildChannel)recievedMessage.Channel).Guild, user);

                    if (resolvedUser == null)
                    {
                        await recievedMessage.Channel.SendMessageAsync("Could not find any user with that name!");
                        return true;
                    }

                    var entry = Mappers.Config.MadUsers.Config.Users.FirstOrDefault(x => x.Server == ((SocketGuildChannel)recievedMessage.Channel).Guild.Id.ToString() && x.User == resolvedUser.Id.ToString());

                    // Stop being mad at the user
                    if (entry != null)
                    {
                        Mappers.Config.MadUsers.Config.Users.Remove(entry);
                        await recievedMessage.Channel.SendMessageAsync($"No longer mad at {resolvedUser.Username}");
                    }
                    else
                    {
                        entry = new MadUser()
                        {
                            Server = Convert.ToString(((SocketGuildChannel)recievedMessage.Channel).Guild.Id),
                            User = Convert.ToString(resolvedUser.Id)
                        };

                        Mappers.Config.MadUsers.Config.Users.Add(entry);
                        await recievedMessage.Channel.SendMessageAsync($"Copy that! I am now mad at {resolvedUser.Username}");
                    }

                    // Save config
                    Mappers.Config.MadUsers.SaveConfig();
                }

                // List mad users
                if (matchedMessage.Groups["MadUserList"].Success)
                {
                    var users = Mappers.Config.MadUsers.Config.Users.Where(x => x.Server == ((SocketGuildChannel)recievedMessage.Channel).Guild.Id.ToString())
                        .Select(y => ((SocketGuildChannel)recievedMessage.Channel).Guild.Users.FirstOrDefault(x => x.Id.ToString() == y.User).Username).ToList();

                    if (users.Count == 0)
                        await recievedMessage.Channel.SendMessageAsync("I am currently not mad at any users");
                    else
                        await recievedMessage.Channel.SendMessageAsync($"I am currently mad at these users:\r\n```\r\n{string.Join("\r\n", users)}\r\n```");
                }

                // List enabled commands on this server
                if (matchedMessage.Groups["EnabledCommandsList"].Success)
                {
                    var server = Mappers.Config.ServerCommands.Config.CommandsConfigServer.FirstOrDefault(x => x.ServerID == ((SocketGuildChannel)recievedMessage.Channel).Guild.Id);
                    if (server == null)
                    {
                        server = new CommandsConfigServer() { ServerID = ((SocketGuildChannel)recievedMessage.Channel).Guild.Id};
                        Mappers.Config.ServerCommands.SetDefaultValues(server);
                        Mappers.Config.ServerCommands.Config.CommandsConfigServer.Add(server);
                        Mappers.Config.ServerCommands.SaveConfig();
                    }

                    var commands = server.Commands;
                    await recievedMessage.Channel.SendMessageAsync($"The following commands are currently enabled on this server:\r\n```\r\n{string.Join("\r\n", commands.Select(x => x.CommandName))}\r\n```");
                }

                // List enabled commands on this server
                if (matchedMessage.Groups["AvailableCommandList"].Success)
                {
                    var commands = Utilities.CommandsUtility.GetAllRegisteredCommands();
                    await recievedMessage.Channel.SendMessageAsync($"The following commands are currently registered and can be used on this server:\r\n```\r\n{string.Join("\r\n", commands.Select(x => x.CommandName))}\r\n```");
                }

                // Enable a command
                if (matchedMessage.Groups["EnableCommandName"].Success)
                {
                    var resolvedCmd = Utilities.CommandsUtility.ResolveCommand(matchedMessage.Groups["EnableCommandName"].Value);
                    if (resolvedCmd != null)
                    {
                        Utilities.CommandsUtility.EnableCommand(((SocketGuildChannel)recievedMessage.Channel).Guild, resolvedCmd);
                        await recievedMessage.Channel.SendMessageAsync($"Command {resolvedCmd.CommandName} is now enabled");
                    }
                    else
                    {
                        await recievedMessage.Channel.SendMessageAsync($"Could not find any command called {matchedMessage.Groups["EnableCommandName"].Value}");
                    }
                }

                // Disables a command
                if (matchedMessage.Groups["DisableCommandName"].Success)
                {
                    var resolvedCmd = Utilities.CommandsUtility.ResolveCommand(matchedMessage.Groups["DisableCommandName"].Value);

                    // AdminCommands may not be disabled
                    if (resolvedCmd.CommandName.ToLower() == "admincommands")
                    {
                        await recievedMessage.Channel.SendMessageAsync($"AdminCommands can not be disabled, or else how are you going to configure me on your server?");
                        return true;
                    }

                    if (resolvedCmd != null)
                    {
                        Utilities.CommandsUtility.DisableCommand(((SocketGuildChannel)recievedMessage.Channel).Guild, resolvedCmd);
                        await recievedMessage.Channel.SendMessageAsync($"Command {resolvedCmd.CommandName} is now disabled");
                    }
                    else
                    {
                        await recievedMessage.Channel.SendMessageAsync($"Could not find any command called {matchedMessage.Groups["DisableCommandName"].Value}");
                    }
                }

                // Enable all the commands
                if (matchedMessage.Groups["EnableAllCommands"].Success)
                {
                    Utilities.CommandsUtility.EnableAllCommands(((SocketGuildChannel)recievedMessage.Channel).Guild);
                    await recievedMessage.Channel.SendMessageAsync("All Commands are now enabled!");
                }

                // List all settings from this server
                if (matchedMessage.Groups["ListSettings"].Success)
                {
                    var configs = Mappers.Config.ServerConfig.Config.Servers.Single(x => x.ServerID == ((SocketGuildChannel) recievedMessage.Channel).Guild.Id).Configs;

                    var output = string.Join("\r\n", configs.Select(x => $"{x.Key.PadLeft(38)}: {x.Value}"));

                    await recievedMessage.Channel.SendMessageAsync($"These settings are available on this server:\r\n```\r\n{output}\r\n```\r\nUse \"setconfig <key> <value>\" to set a configuration value");
                }

                // Set new condfig value
                if (matchedMessage.Groups["SetConfigKey"].Success)
                {
                    var configEntry =
                        Mappers.Config.ServerConfig.Config.Servers.Single(x => x.ServerID == ((SocketGuildChannel) recievedMessage.Channel).Guild.Id)
                            .Configs.SingleOrDefault(x => x.Key == matchedMessage.Groups["SetConfigKey"].Value);
                    if (configEntry == null)
                    {
                        await recievedMessage.Channel.SendMessageAsync($"There is no setting with key {matchedMessage.Groups["SetConfigKey"].Value}");
                        return true;
                    }

                    configEntry.Value = matchedMessage.Groups["SetConfigValue"].Value;
                    Mappers.Config.ServerConfig.SaveConfig();

                    await recievedMessage.Channel.SendMessageAsync("Value set");
                }

                return true;
            }
            else
            {
                await recievedMessage.Channel.SendMessageAsync("UNAUTHORIZED ACCESS DETECTED!\r\nBut seriously, this command is only for admins (and you are not one of them ...)!");
                return true;
            }
        }
    }
}
