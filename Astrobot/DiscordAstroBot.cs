using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DiscordAstroBot.Helpers;
using DiscordAstroBot.Objects.Config;

namespace DiscordAstroBot
{
    public class DiscordAstroBot
    {
        /// <summary>
        /// A list of all the servers where the bot anounced that he is now running
        /// </summary>
        private List<ulong> HailedServers { get; set; } = new List<ulong>();

        /// <summary>
        /// The Discord client used to talk to the Discord API
        /// </summary>
        DiscordSocketClient DiscordClient { get; set; }

        /// <summary>
        /// Holds all the registered Commands
        /// </summary>
        public static List<Command> Commands { get; set; }

        /// <summary>
        /// The prefix that this bot listens to
        /// </summary>
        public string ChatPrefix { get; set; }

        /// <summary>
        /// Constructor of the bot, this is where all the initialisations happen
        /// </summary>
        /// <param name="token"></param>
        /// <param name="chatPrefix"></param>
        public DiscordAstroBot()
        {
            // Initialize config store
            Mappers.Config.ServerCommands.LoadConfig();
            Mappers.Config.MadUsers.LoadConfig();
            Mappers.Config.ServerConfig.LoadConfig();
        }

        public async void InitDiscordClient(string token, string chatPrefix)
        {
            // Initialize the client
            Log<DiscordAstroBot>.InfoFormat("Login into Discord");
            DiscordClient = new DiscordSocketClient(/*x =>
            {
                x.AppName = "Discord Astro Bot";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            }*/);

            DiscordClient.Log += Log;
            await DiscordClient.LoginAsync(TokenType.Bot, token);
            await DiscordClient.StartAsync();

            this.ChatPrefix = chatPrefix;

            RegisterCommands();

            DiscordClient.MessageReceived += MessageReceived;
            DiscordClient.GuildAvailable += DiscordClient_ServerAvailable;
            DiscordClient.GuildMemberUpdated += DiscordClient_UserUpdated;
            DiscordClient.UserJoined += DiscordClient_UserJoined;
            DiscordClient.JoinedGuild += DiscordClient_JoinedServer;

            Log<DiscordAstroBot>.InfoFormat("Login successfull");
        }

        /// <summary>
        /// When the bot joins a new server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task DiscordClient_JoinedServer(SocketGuild server)
        {
            server.DefaultChannel.SendMessageAsync("Yay! I got invited to a new server!\r\nHello everyone!");

            SetupDefaultSettings(server);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Initializes default settings store for newly joined servers
        /// </summary>
        /// <param name="dserver"></param>
        /// <returns></returns>
        private Task SetupDefaultSettings(SocketGuild dserver)
        {
            // Setup default server commands config
            var server = Mappers.Config.ServerCommands.Config.CommandsConfigServer.FirstOrDefault(x => x.ServerID == dserver.Id);
            if (server == null)
            {
                server = new CommandsConfigServer() { ServerID = dserver.Id };
                Mappers.Config.ServerCommands.SetDefaultValues(server);
                Mappers.Config.ServerCommands.Config.CommandsConfigServer.Add(server);
                Mappers.Config.ServerCommands.SaveConfig();
            }

            // Setup default server config
            var serverCfg = Mappers.Config.ServerConfig.Config.Servers.FirstOrDefault(x => x.ServerID == dserver.Id);
            if (serverCfg == null)
            {
                serverCfg = new ServerSettingsServer() { ServerID = dserver.Id };
                Mappers.Config.ServerConfig.SetDefaultValues(serverCfg);
                Mappers.Config.ServerConfig.Config.Servers.Add(serverCfg);
                Mappers.Config.ServerConfig.SaveConfig();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets called when a user joins the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task DiscordClient_UserJoined(SocketGuildUser user)
        {
            Log<DiscordAstroBot>.InfoFormat($"New user {user.Username} joined on server {user.Username}");

            // Send a welcome message in the default channel
            var rulesChannel = user.Guild.Channels.FirstOrDefault(x => x.Name.ToLower() == "rules");
            if (rulesChannel != null)
            {
                user.Guild.DefaultChannel.SendMessageAsync($"A new user joined! Say hi to {user.Mention}\r\nMake sure to check out the {((ITextChannel)rulesChannel).Mention} channel!");
            }
            else
            {
                user.Guild.DefaultChannel.SendMessageAsync($"A new user joined! Say hi to {user.Mention}");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets called when a user update happens (for example when he comes online)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task DiscordClient_UserUpdated(SocketGuildUser beforeUser, SocketGuildUser afterUser)
        {
            // If astrobots best friend comes online (another bot) hail it
            if (afterUser.Username.ToLower().Contains("eta") || afterUser.Username.ToLower().Contains("gaben") && afterUser.IsBot)
            {
                if (beforeUser.Status != UserStatus.Online && afterUser.Status == UserStatus.Online)
                {
                    ReactionsHelper.HailEta(beforeUser.Guild, afterUser);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Event that gets raised when a new server becomes available for the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task DiscordClient_ServerAvailable(SocketGuild server)
        {
            // Since random disconnects and reconects to the servers happen, we dont want
            // the bot to tell everyone that he is online everytime this happen,
            // but rather only the first time
            if (!HailedServers.Contains(server.Id /*false*/))
            {
                //<e.Server.DefaultChannel.SendMessage("I am now up and running");
                HailedServers.Add(server.Id);
                SetupDefaultSettings(server);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Listens for commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task MessageReceived(SocketMessage recievedMessage)
        {
            try
            {
                // Check to make sure that the bot is not the author
                if (recievedMessage.Author.Id != DiscordClient.CurrentUser.Id)
                {
                    var splitted = recievedMessage.Content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length > 0 && splitted[0].ToLower() == this.ChatPrefix || recievedMessage.MentionedUsers.Any(x => x.Id == DiscordClient.CurrentUser.Id))
                    {
                        Log<DiscordAstroBot>.InfoFormat("Message recieved: {0}", recievedMessage.Content);

                        // Search for synonyms usind regex                       
                        var message = recievedMessage.Content.Replace(ChatPrefix, "").Replace(DiscordClient.CurrentUser.Mention, "").Trim();
                        Task.Run(() =>
                        {
                            // Set threading culture for parsing floating numbers
                            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                            bool commandExecuted = false;

                            foreach (var command in Commands)
                            {
                                // if Command is disabled on this server, ignore it
                                if (!Mappers.Config.ServerCommands.Config.CommandsConfigServer.First(x => x.ServerID == ((SocketTextChannel)recievedMessage.Channel).Guild.Id).Commands.Any(x => x.CommandName.ToLower() == command.CommandName.ToLower() && x.Enabled))
                                    continue;

                                // Check if there is a synonym that matches the message
                                foreach (var synonym in command.CommandSynonyms)
                                {
                                    var regex = new Regex(synonym, RegexOptions.IgnoreCase);
                                    if (regex.IsMatch(message))
                                    {
                                        var match = regex.Match(message);

                                        try
                                        {
                                            commandExecuted = command.MessageRecieved(match, recievedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            recievedMessage.Channel.SendMessageAsync(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                                            Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", recievedMessage.Content, ex.Message);
                                        }

                                        if (commandExecuted)
                                            break;
                                    }
                                }
                                if (commandExecuted)
                                    break;
                            }

                            // If no synonym found execute smalltalk
                            if (!commandExecuted)
                            {
                                var smallTalkCommand = Commands.FirstOrDefault(x => x.CommandName == "SmallTalk");

                                try
                                {
                                    smallTalkCommand.MessageRecieved(new Regex(".*").Match(message), recievedMessage);
                                }
                                catch (Exception ex)
                                {
                                    recievedMessage.Channel.SendMessageAsync(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                                    Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", recievedMessage.Content, ex.Message);
                                }
                                commandExecuted = true;
                            }
                        });
                    }
                    else
                    {
                        var reaction = ReactionsHelper.ReactToNonTag(recievedMessage.Content);
                        if (!string.IsNullOrEmpty(reaction))
                            recievedMessage.Channel.SendMessageAsync(reaction);
                    }
                }
            }
            catch (Exception ex)
            {
                recievedMessage.Channel.SendMessageAsync(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", recievedMessage.Content, ex.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers all the commands
        /// </summary>
        private void RegisterCommands()
        {
            Log<DiscordAstroBot>.InfoFormat("Registering commands...");

            Commands = new List<Command>();

            // Add Commands
            Commands.Add(new Commands.AdminCommands());
            Commands.Add(new Commands.SmallTalkCommand());
            Commands.Add(new Commands.TestCommand());
            Commands.Add(new Commands.AstroMetry());
            Commands.Add(new Commands.GeoLocation());
            Commands.Add(new Commands.Weather());
            Commands.Add(new Commands.Launches());
            Commands.Add(new Commands.Simbad());
            Commands.Add(new Commands.Version());
            Commands.Add(new Commands.DSSCommand());

            foreach (var command in Commands)
            {
                Log<DiscordAstroBot>.InfoFormat("Command registered \"{0}\"", command.CommandName);
            }
        }

        /// <summary>
        /// Logs any messages that come from the DiscordClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Debug:
                    Log<DiscordAstroBot>.DebugFormat(msg.Message);
                    break;
                case LogSeverity.Error:
                    Log<DiscordAstroBot>.ErrorFormat(msg.Message);
                    break;
                case LogSeverity.Info:
                    Log<DiscordAstroBot>.InfoFormat(msg.Message);
                    break;
                case LogSeverity.Verbose:
                    Log<DiscordAstroBot>.DebugFormat(msg.Message);
                    break;
                case LogSeverity.Warning:
                    Log<DiscordAstroBot>.WarnFormat(msg.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
