using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Objects.Config;

namespace DiscordAstroBot.Mappers.Config
{
    /// <summary>
    /// Mapper for the server commands config
    /// Acts as a proxy class to define default settings
    /// </summary>
    public static class MadUsers
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        public const string XmlFile = "config/MadUsers.xml";

        /// <summary>
        /// The default per server config
        /// </summary>
        public static List<MadUser> DefaultConfig => new List<MadUser>()
        {

        };

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static Objects.Config.MadUsers Config { get; set; }

        /// <summary>
        /// Loads the config
        /// </summary>
        public static void LoadConfig()
        {
            Config = XmlSerialization.XmlStateController.LoadObject<Objects.Config.MadUsers>(XmlFile);

            // Default values
            SetDefaultValues(Config);
        }

        /// <summary>
        /// Sets the default values on a given server
        /// </summary>
        /// <param name="serverCfg"></param>
        public static void SetDefaultValues(Objects.Config.MadUsers config)
        {
            foreach (var defaultValue in DefaultConfig)
            {
                if (!config.Users.Any(x => x.Server == defaultValue.Server && x.User == defaultValue.User))
                    config.Users.Add(defaultValue);
            }
        }

        /// <summary>
        /// Saves the config to the xml file
        /// </summary>
        public static void SaveConfig()
        {
            XmlSerialization.XmlStateController.SaveObject(Config, XmlFile);
        }
    }
}
