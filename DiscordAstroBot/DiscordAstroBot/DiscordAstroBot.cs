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
        List<Command> Commands { get; set; }

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
            // Check to make sure that the bot is not the author
            if (!e.Message.IsAuthor)
            {
                var splitted = e.Message.RawText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (splitted[0].ToLower() == "astrobot")
                {
                    Log<DiscordAstroBot>.InfoFormat("Message recieved: {0}", e.Message.Text);

                    // If no command given, show info and available commands
                    if (splitted.Length == 1)
                    {
                        string availableCommands = "";
                        foreach (var command in Commands)
                        {
                            availableCommands = string.Format("{0} {1}", availableCommands, command.CommandName);
                        }
                        e.Channel.SendMessage(string.Format("AstroBot, at your service! Available commands: {0}", availableCommands));
                    }
                    // Else search for the command
                    else
                    {
                        Command command = this.Commands.FirstOrDefault(x => x.CommandName == splitted[1]);
                        if (command == null)
                        {
                            e.Channel.SendMessage(string.Format("Uknown Command: {0}", splitted[1]));
                        }
                        else
                        {
                            command.MessageRecieved(string.Join(" ", splitted.Skip(2).Take(splitted.Length - 2).ToArray()), e);
                        }
                    }
                }
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
            Commands.Add(new Commands.TestCommand());

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
