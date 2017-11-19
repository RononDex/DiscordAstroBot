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
using System.Timers;
using WhiteList = DiscordAstroBot.Mappers.Config.WhiteList;
using DiscordAstroBot.Utilities;

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
        /// Holds all the timer jobs
        /// </summary>
        public static List<TimerJobs.TimerJobBase> TimerJobs { get; set; } = new List<TimerJobs.TimerJobBase>();

        /// <summary>
        /// The prefix that this bot listens to
        /// </summary>
        public string ChatPrefix { get; set; }

        /// <summary>
        /// If whitelist is enabled, only servers on the whitelist may use the bot
        /// (measure to limit hardware usage of bot to only whitelisted servers)
        /// </summary>
        public bool WhiteListEnabled { get; set; }

        /// <summary>
        /// The name of the bot owner, used to tell people how to contact the owner if needed
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// Timer that executes the timer jobs
        /// </summary>
        public System.Timers.Timer TimerJobTimer { get; set; }

        /// <summary>
        /// Constructor of the bot, this is where all the initialisations happen
        /// </summary>
        /// <param name="token"></param>
        /// <param name="chatPrefix"></param>
        public DiscordAstroBot()
        {
            try
            {
                // Initialize config store
                Mappers.Config.ServerCommands.LoadConfig();
                Mappers.Config.MadUsers.LoadConfig();
                Mappers.Config.ServerConfig.LoadConfig();
                Mappers.Config.WhiteList.LoadConfig();
                Mappers.Config.TimerJobExecutions.LoadConfig();
            }
            catch (Exception ex)
            {
                Log<DiscordAstroBot>.ErrorFormat($"Error loading config: {ex.Message}");
            }
        }

        public async Task InitDiscordClient(string token, string chatPrefix)
        {
            // Initialize the client
            Log<DiscordAstroBot>.InfoFormat("Login into Discord");
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 150,                
            });

            DiscordClient.Log += Log;
            await DiscordClient.LoginAsync(TokenType.Bot, token);
            await DiscordClient.StartAsync();

            this.ChatPrefix = chatPrefix;

            RegisterCommands();
            SetupTimerJobs();

            DiscordClient.MessageReceived += MessageReceived;
            DiscordClient.GuildAvailable += DiscordClient_ServerAvailable;
            DiscordClient.GuildMemberUpdated += DiscordClient_UserUpdated;
            DiscordClient.UserJoined += DiscordClient_UserJoined;
            DiscordClient.JoinedGuild += DiscordClient_JoinedServer;
            DiscordClient.MessageDeleted += DiscordClient_MessageDeleted;
            DiscordClient.UserLeft += DiscordClient_UserLeft;
            DiscordClient.UserBanned += DiscordClient_UserBanned;

            Log<DiscordAstroBot>.InfoFormat("Login successfull");
        }

        /// <summary>
        /// When a user gets banned
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        private Task DiscordClient_UserBanned(SocketUser arg1, SocketGuild arg2)
        {
            Log<DiscordAstroBot>.Info($"User {arg1.Username} got banned on server {arg2.Name})");

            DiscordUtility.LogToDiscord($"User {arg1.Username} got **banned** from the server!", arg2);

            return Task.CompletedTask;
        }

        /// <summary>
        /// When a user leaves a server
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task DiscordClient_UserLeft(SocketGuildUser arg)
        {
            Log<DiscordAstroBot>.Info($"User {arg.Username} left on server {arg.Guild.Name})");

            DiscordUtility.LogToDiscord($"Oh no! User {arg.Username} **left** the server!\r\nHe / She was on the server since {arg.JoinedAt}", arg.Guild);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets called when a user deletes a messages
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        private Task DiscordClient_MessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (arg2 is IGuildChannel)
            {
                Task.Run(async () =>
                {
                    // Log deleted message
                    var channel = arg2 as IGuildChannel;
                    var msg = arg1.Value;
                    if (arg1.Value == null)
                    {
                        msg = await arg2.GetMessageAsync(arg1.Id);
                    }

                    // If the message is not cached by the bot, there is no way to recover it
                    if (msg != null)
                    {
                        Log<DiscordAstroBot>.Warn($"DELETED MESSAGE: The following message was deleted in channel {channel.Name} on server {channel.Guild.Name}:\r\nAuthor:{arg1.Value.Author.Username}\r\n{arg1.Value.Content}");

                        // Log message into discord
                        Utilities.DiscordUtility.LogToDiscord($"__**WARNING:**__: Following message was deleted in channel {channel.Name}:\r\n```\r\nAuthor: {arg1.Value.Author.Username}\r\n{arg1.Value.Content}```", channel.Guild);
                    }
                    else
                    {
                        Log<DiscordAstroBot>.Warn($"DELETED MESSAGE in channel {channel.Name} on server {channel.Guild.Name}");

                        // Log message into discord
                        Utilities.DiscordUtility.LogToDiscord($"__**WARNING:**__: Deleted message in channel {channel.Name}", channel.Guild);
                    }
                });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Setups the TimerJobs
        /// </summary>
        private void SetupTimerJobs()
        {
            // Register the timerjobs
            Log<DiscordAstroBot>.InfoFormat("Registering TimerJobs...");

            TimerJobs.Add(new TimerJobs.LaunchNews());
            TimerJobs.Add(new TimerJobs.IntermediateLaunchNotify());

            foreach (var job in TimerJobs)
            {
                Log<DiscordAstroBot>.InfoFormat($"TimerJob registered \"{job.Name}\", Next execution: {job?.NextExecutionTime}");
            }

            // Start the timer that will check periodically if a job has to be executed
            TimerJobTimer = new System.Timers.Timer(1000 * 60);
            TimerJobTimer.Elapsed += ExecuteTimerJobs;
            TimerJobTimer.Start();
        }

        /// <summary>
        /// Event gets called when bot checks for timer jobs that have to run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteTimerJobs(object sender, ElapsedEventArgs e)
        {
            foreach (var job in TimerJobs)
            {
                try
                {
                    // If the job has to be executed, execute it on all enabled servers
                    if (job.NextExecutionTime < DateTime.Now)
                    {
                        foreach (var server in this.DiscordClient.Guilds)
                        {
                            try
                            {
                                Log<DiscordAstroBot>.Info($"Executing TimerJob {job.Name} on server {server.Name}");
                                job.Execute(server);
                                job.LastExecutionTime = DateTime.Now;
                            }
                            catch (Exception ex)
                            {
                                Log<DiscordAstroBot>.Error($"Error while executing timer job {job.Name}: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log<DiscordAstroBot>.Error($"Error executing TimerJob {job.Name}: {ex.Message}");
                }
            }
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


            // If Whitelist is enabled and server is not on white list
            if (WhiteListEnabled && WhiteList.WhitelistedServers.Entries.All(x => x.ServerID != ((SocketTextChannel)server.DefaultChannel).Guild.Id))
            {
                WriteNotOnWhitelistResponse(server.DefaultChannel);
                return Task.CompletedTask;
            }

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
                Mappers.Config.ServerCommands.Config.CommandsConfigServer.Add(server);
            }

            Mappers.Config.ServerCommands.SetDefaultValues(server);
            Mappers.Config.ServerCommands.SaveConfig();

            // Setup default server config
            var serverCfg = Mappers.Config.ServerConfig.Config.Servers.FirstOrDefault(x => x.ServerID == dserver.Id);
            if (serverCfg == null)
            {
                serverCfg = new ServerSettingsServer() { ServerID = dserver.Id };
                Mappers.Config.ServerConfig.Config.Servers.Add(serverCfg);
            }

            Mappers.Config.ServerConfig.SetDefaultValues(serverCfg);
            Mappers.Config.ServerConfig.SaveConfig();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets called when a user joins the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private Task DiscordClient_UserJoined(SocketGuildUser user)
        {
            Log<DiscordAstroBot>.InfoFormat($"New user {user.Username} joined on server {user.Guild.Name}");

            // If Whitelist is enabled and server is not on white list
            if (WhiteListEnabled && WhiteList.WhitelistedServers.Entries.All(x => x.ServerID != user.Guild.Id))
            {
                return Task.CompletedTask;
            }

            // Send a welcome message in the default channel
            if (Mappers.Config.ServerConfig.Config.Servers.Single(x => x.ServerID == user.Guild.Id).Configs.Any(x => x.Key == "HailNewUsers" && bool.Parse(x.Value)))
            {
                var rulesChannel = user.Guild.Channels.FirstOrDefault(x => x.Name != null && x.Name.ToLower().Contains("rules"));
                if (rulesChannel != null)
                {
                    user.Guild.DefaultChannel.SendMessageAsync($"A new user joined! Say hi to {user.Mention}\r\nMake sure to check out the {((ITextChannel)rulesChannel).Mention} channel!");
                }
                else
                {
                    user.Guild.DefaultChannel.SendMessageAsync($"A new user joined! Say hi to {user.Mention}");
                }
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
            if (!string.IsNullOrEmpty(afterUser.Username) && afterUser.Username.ToLower().Contains("eta") && afterUser.IsBot)
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
                Log<DiscordAstroBot>.InfoFormat("Server available:  ServerName:  {0},   ServerID:  {1}", server.Name.PadRight(50, ' '), server.Id);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Show message that server is not whitelisted
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private async Task WriteNotOnWhitelistResponse(ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync($"I am currently disabled on this server, since I am configured to run on WhiteList mode and this server is not on the WhiteList");
            await channel.SendMessageAsync($"Please contact the programmer / owner of this bot ({OwnerName}) if you wish to use me on your server and ask to get your server WhiteListed");
            await channel.SendMessageAsync($"This is to limit hardware usage on the physical server where I am running on.");
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
                    // Don't allow direct message for now, since this would require some extra code to handle them properly
                    if (recievedMessage.Channel is IPrivateChannel)
                    {
                        recievedMessage.Channel.SendMessageAsync("Due to technical limiations I can't handle direct messages at the moment.");
                        return Task.CompletedTask;
                    }

                    // If Whitelist is enabled and server is not on white list
                    if (WhiteListEnabled && WhiteList.WhitelistedServers.Entries.All(x => x.ServerID != ((SocketTextChannel)recievedMessage.Channel).Guild.Id))
                    {
                        WriteNotOnWhitelistResponse(recievedMessage.Channel);
                        return Task.CompletedTask;
                    }

                    var splitted = recievedMessage.Content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length > 0 && splitted[0].ToLower() == this.ChatPrefix || recievedMessage.MentionedUsers.Any(x => x.Id == DiscordClient.CurrentUser.Id))
                    {
                        Log<DiscordAstroBot>.InfoFormat("Message recieved: {0}", recievedMessage.Content);

                        // Search for synonyms usind regex                       
                        var message = recievedMessage.Content.Replace(ChatPrefix, "").Replace(DiscordClient.CurrentUser.Mention.Replace("!", ""), "").Replace(DiscordClient.CurrentUser.Mention, "").Trim();
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
                                            commandExecuted = command.MessageRecieved(match, recievedMessage).Result;
                                        }
                                        catch (Exception ex)
                                        {
                                            recievedMessage.Channel.SendMessageAsync($"Oh noes! Something you did caused me to crash: {ex.Message}");
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
                                    recievedMessage.Channel.SendMessageAsync($"Oh noes! Something you did caused me to crash: {ex.Message}");
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
                recievedMessage.Channel.SendMessageAsync($"Oh noes! Something you did caused me to crash: {ex.Message}");
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
            Commands.Add(new Commands.AstroMetry());
            Commands.Add(new Commands.GeoLocation());
            Commands.Add(new Commands.Weather());
            Commands.Add(new Commands.Launches());
            Commands.Add(new Commands.Simbad());
            Commands.Add(new Commands.Version());
            Commands.Add(new Commands.DSSCommand());
            Commands.Add(new Commands.UserCommands());
            Commands.Add(new Commands.Help());
            Commands.Add(new Commands.TestCommand());

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
