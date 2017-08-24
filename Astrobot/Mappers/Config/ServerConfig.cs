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
    public static class ServerConfig
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        public const string XmlFile = "config/ServerSettings.xml";

        /// <summary>
        /// The default per server config
        /// </summary>
        public static List<Objects.Config.ServerSettingsServerConfig> DefaultConfig => new List<ServerSettingsServerConfig>()
        {
            new ServerSettingsServerConfig() { Key = "HailNewUsers", Value = "false" },
        };

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static ServerSettings Config { get; set; }

        /// <summary>
        /// Loads the config
        /// </summary>
        public static void LoadConfig()
        {
            Config = XmlSerialization.XmlStateController.LoadObject<ServerSettings>(XmlFile);

            // Default values
            foreach (var server in Config.Servers)
                SetDefaultValues(server);
        }

        /// <summary>
        /// Sets the default values on a given server
        /// </summary>
        /// <param name="serverCfg"></param>
        public static void SetDefaultValues(ServerSettingsServer serverCfg)
        {
            foreach (var defaultValue in DefaultConfig)
            {
                if (!serverCfg.Configs.Any(x => x.Key == defaultValue.Key))
                    serverCfg.Configs.Add(defaultValue);
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
