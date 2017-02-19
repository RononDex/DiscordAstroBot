using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordAstroBot
{
    public class DiscordAstroBot
    {
        /// <summary>
        /// A list of all the servers where the bot anounced that he is now running
        /// </summary>
        private List<ulong> HailedServers { get; set; } = new List<ulong>();


        DiscordClient DiscordClient { get; set; }

        /// <summary>
        /// Holds all the registered Commands
        /// </summary>
        public List<Command> Commands { get; set; }

        /// <summary>
        /// The prefix that this bot listens to
        /// </summary>
        public string ChatPrefix { get; set; }

        /// <summary>
        /// Constructor of the bot, this is where all the initialisations happen
        /// </summary>
        /// <param name="token"></param>
        /// <param name="chatPrefix"></param>
        public DiscordAstroBot(string token, string chatPrefix)
        {
            // Initialize the client
            Log<DiscordAstroBot>.InfoFormat("Loging into Discord");
            DiscordClient = new DiscordClient(x =>
            {
                x.AppName = "Discord Astro Bot";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            this.ChatPrefix = chatPrefix;

            RegisterCommands();

            DiscordClient.MessageReceived += MessageReceived;
            DiscordClient.ServerAvailable += DiscordClient_ServerAvailable;
            DiscordClient.UserUpdated += DiscordClient_UserUpdated;
            DiscordClient.UserJoined += DiscordClient_UserJoined;

            // Login into Discord
            DiscordClient.ExecuteAndWait(async () =>
            {
                await DiscordClient.Connect(token, TokenType.Bot);
                Log<DiscordAstroBot>.InfoFormat("Login successfull");
            });
        }
        /// <summary>
        /// Gets called when a user joins the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscordClient_UserJoined(object sender, UserEventArgs e)
        {
            Log<DiscordAstroBot>.InfoFormat($"New user {e.User.Name} joined on server {e.Server.Name}");

            // Send a welcome message in the default channel
            var rulesChannel = e.Server.AllChannels.FirstOrDefault(x => x.Name.ToLower() == "rules");
            if (rulesChannel != null)
            {
                e.Server.DefaultChannel.SendMessage($"A new user joined! Say hi to {e.User.Mention}\r\nMake sure to check out the {rulesChannel.Mention} channel!");
            }
            else
            {
                e.Server.DefaultChannel.SendMessage($"A new user joined! Say hi to {e.User.Mention}");
            }
        }

        /// <summary>
        /// Gets called when a user update happens (for example when he comes online)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscordClient_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            // If astrobots best friend comes online (another bot) hail it
            if (e.After.Name.ToLower().Contains("eta") && e.After.IsBot)
            {
                if (e.Before.Status.Value.ToLower() != "online" && e.After.Status.Value.ToLower() == "online")
                {
                    Reactions.Reactions.HailEta(e.Server, e.After);
                }
            }
        }

        /// <summary>
        /// Event that gets raised when a new server becomes available for the bot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscordClient_ServerAvailable(object sender, ServerEventArgs e)
        {
            // Since random disconnects and reconects to the servers happen, we dont want
            // the bot to tell everyone that he is online everytime this happen,
            // but rather only the first time
            if (!HailedServers.Contains(e.Server.Id /*false*/))
            {
                //<e.Server.DefaultChannel.SendMessage("I am now up and running");
                HailedServers.Add(e.Server.Id);
            }
        }

        /// <summary>
        /// Listens for commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                // Check to make sure that the bot is not the author
                if (!e.Message.IsAuthor)
                {
                    var splitted = e.Message.RawText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length > 0 && splitted[0].ToLower() == this.ChatPrefix || e.Message.MentionedUsers.Any(x => x.Id == DiscordClient.CurrentUser.Id))
                    {
                        Log<DiscordAstroBot>.InfoFormat("Message recieved: {0}", e.Message.Text);

                        // Search for synonyms usind regex
                        bool commandExecuted = false;
                        var message = e.Message.RawText.Replace(ChatPrefix, "").Replace(DiscordClient.CurrentUser.Mention, "").Trim();
                        foreach (var command in this.Commands)
                        {
                            foreach (var synonym in command.CommandSynonyms)
                            {
                                var regex = new Regex(synonym, RegexOptions.IgnoreCase);
                                if (regex.IsMatch(message))
                                {
                                    var match = regex.Match(message);
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            command.MessageRecieved(match, e);
                                        }
                                        catch (Exception ex)
                                        {
                                            e.Channel.SendMessage(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                                            Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", e.Message.RawText, ex.Message);
                                        }
                                    });
                                    commandExecuted = true;
                                    break;
                                }
                            }
                        }

                        // If no synonym found execute smalltalk
                        if (!commandExecuted)
                        {
                            var smallTalkCommand = this.Commands.FirstOrDefault(x => x.CommandName == "SmallTalk");
                            Task.Run(() =>
                            {
                                try
                                {
                                    smallTalkCommand.MessageRecieved(new Regex(".*").Match(message), e);
                                }
                                catch (Exception ex)
                                {
                                    e.Channel.SendMessage(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                                    Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", e.Message.RawText, ex.Message);
                                }
                            });
                            commandExecuted = true;
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                e.Channel.SendMessage(string.Format("Oh noes! Something you did caused me to crash: {0}", ex.Message));
                Log<DiscordAstroBot>.ErrorFormat("Error for message: {0}: {1}", e.Message.RawText, ex.Message);
            }
        }

        /// <summary>
        /// Registers all the commands
        /// </summary>
        private void RegisterCommands()
        {
            Log<DiscordAstroBot>.InfoFormat("Registering commands...");

            Commands = new List<Command>();

            // Add Commands
            Commands.Add(new Commands.SmallTalkCommand());
            Commands.Add(new Commands.TestCommand());
            Commands.Add(new Commands.AstroMetry());
            Commands.Add(new Commands.GeoLocation());
            Commands.Add(new Commands.Weather());
            Commands.Add(new Commands.Launches());
            Commands.Add(new Commands.Simbad());
            Commands.Add(new Commands.Version());

            foreach (var command in this.Commands)
            {
                Log<DiscordAstroBot>.InfoFormat("Command registered \"{0}\"", command.CommandName);
            }
        }

        /// <summary>
        /// Logs any messages that come from the DiscordClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log(object sender, LogMessageEventArgs e)
        {
            switch (e.Severity)
            {
                case LogSeverity.Debug:
                    Log<DiscordAstroBot>.DebugFormat(e.Message);
                    break;
                case LogSeverity.Error:
                    Log<DiscordAstroBot>.ErrorFormat(e.Message);
                    break;
                case LogSeverity.Info:
                    Log<DiscordAstroBot>.InfoFormat(e.Message);
                    break;
                case LogSeverity.Verbose:
                    Log<DiscordAstroBot>.DebugFormat(e.Message);
                    break;
                case LogSeverity.Warning:
                    Log<DiscordAstroBot>.WarnFormat(e.Message);
                    break;
            }
        }
    }
}
