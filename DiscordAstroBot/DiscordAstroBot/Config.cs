using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Status.Rest;
using DiscordAstroBot.Objects.Config;

namespace DiscordAstroBot
{
    public static class Config
    {
        /// <summary>
        /// Initializes the config store from the xml files
        /// </summary>
        public static void Initialize()
        {
            Log<DiscordAstroBot>.Info($"Loading config files...");

            MadUsers = XmlSerialization.XmlStateController.LoadObject<MadUsers>("config/MadUsers.xml");
            CommandsConfig = XmlSerialization.XmlStateController.LoadObject<CommandsConfig>("config/Commands.xml");

            Log<DiscordAstroBot>.Info($"Config loaded");
        }

        /// <summary>
        /// The current users that the bot is mad at
        /// </summary>
        public static MadUsers MadUsers { get; set; }

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static CommandsConfig CommandsConfig { get; set; }
    }
}
