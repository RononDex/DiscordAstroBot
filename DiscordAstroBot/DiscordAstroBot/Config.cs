using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API.Status.Rest;
using DiscordAstroBot.Objects;

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

            MadUsers = XmlSerialization.XmlStateController.LoadObject<MadUsers>("configs/MadUsers.xml");

            Log<DiscordAstroBot>.Info($"Config loaded");
        }

        public static MadUsers MadUsers { get; set; }
    }
}
