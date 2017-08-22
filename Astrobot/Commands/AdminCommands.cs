﻿using System;
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

        public override string[] CommandSynonyms => new[]
        {
            @"toggle mad mode for (?'MadUser'.*)",
            @"who are you mad at(?'MadUserList')(\?)?",
            @"(what|which) commands are enabled(?'EnabledCommandsList')( on this server)?(\?)?",
            @"(what|which) commands are (available|registered)(?'AvailableCommandList')( on this server)?(\?)?",
            @"enable all commands(?'EnableAllCommands')",
            @"enable( command)? (?'EnableCommandName'[^\s]+)",
            @"disable( command)? (?'DisableCommandName'[^\s]+)",
        };

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

                return true;
            }
            else
            {
                await recievedMessage.Channel.SendMessageAsync("UNAUTHORIZED ACCESS DETECTED!\r\nBut seriously, this command is only for admins (and you are not one of them, so...)!");
                return true;
            }
        }
    }
}
