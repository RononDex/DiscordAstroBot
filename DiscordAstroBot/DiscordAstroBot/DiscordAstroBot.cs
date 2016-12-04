using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot
{
    public class DiscordAstroBot
    {
        /// <summary>
        /// The DiscordClient
        /// </summary>
        DiscordClient DiscordClient { get; set; }

        /// <summary>
        /// Holds all the registered Commands
        /// </summary>
        public List<Command> Commands { get; set; }

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

            // Setting up chat bot config
            this.DiscordClient.UsingCommands(x =>
            {
                x.PrefixChar = Convert.ToChar(chatPrefix);
                x.HelpMode = HelpMode.Public;
            });

            RegisterCommands();

            DiscordClient.MessageReceived += MessageReceived;

            // Login into Discord
            DiscordClient.ExecuteAndWait(async () =>
            {
                await DiscordClient.Connect(token, TokenType.Bot);
                Log<DiscordAstroBot>.InfoFormat("Login successfull");
            });
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
                    if (splitted.Length > 0 && splitted[0].ToLower() == "astrobot")
                    {
                        Log<DiscordAstroBot>.InfoFormat("Message recieved: {0}", e.Message.Text);

                        // If no command given, use Smalltalk command
                        if (splitted.Length == 1)
                        {
                            var smallTalkCommand = this.Commands.FirstOrDefault(x => x.CommandName == "SmallTalk");
                            smallTalkCommand.MessageRecieved(string.Join(" ", splitted.Skip(1).Take(splitted.Length - 1).ToArray()), e);
                        }
                        // Execute selected command
                        else if (splitted.Length >= 2 && this.Commands.FirstOrDefault(x => x.CommandName.ToLower() == splitted[1].ToLower()) != null)
                        {
                            var command = this.Commands.FirstOrDefault(x => x.CommandName.ToLower() == splitted[1].ToLower());
                            command.MessageRecieved(string.Join(" ", splitted.Skip(2).Take(splitted.Length - 2).ToArray()), e);
                        }
                        // If no command found with this name, search for synonyms or redirect to smalltalk command
                        else
                        {
                            // Search for synonyms
                            var synonymCommand = this.Commands.FirstOrDefault(x => x.CommandSynonyms.Any(y => string.Join(" ", splitted.Skip(1).Take(splitted.Length - 1).ToArray()).StartsWith(y)));

                            bool commandExecuted = false;

                            foreach (var command in this.Commands)
                            {
                                var synonym = command.CommandSynonyms.FirstOrDefault(x => string.Join(" ", splitted.Skip(1).Take(splitted.Length - 1).ToArray()).ToLower().StartsWith(x.ToLower()));
                                if (synonym != null)
                                {
                                    synonymCommand.MessageRecieved(e.Message.RawText.Replace(synonym + " ", string.Empty), e);
                                    commandExecuted = true;
                                    break;
                                }
                            }

                            // If no synonym found execute smalltalk
                            if (!commandExecuted)
                            {
                                var smallTalkCommand = this.Commands.FirstOrDefault(x => x.CommandName == "SmallTalk");
                                smallTalkCommand.MessageRecieved(string.Join(" ", splitted.Skip(1).Take(splitted.Length - 1).ToArray()), e);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
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
            var commandService = DiscordClient.GetService<CommandService>();

            Commands = new List<Command>();

            // Add Commands
            Commands.Add(new Commands.SmallTalkCommand());
            Commands.Add(new Commands.TestCommand());
            Commands.Add(new Commands.AstroMetry());
            Commands.Add(new Commands.GeoLocation());

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
