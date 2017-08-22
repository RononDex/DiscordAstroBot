using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Mappers.Config
{
    /// <summary>
    /// Mapper for the whitelist config file
    /// </summary>
    public static class WhiteList
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        public const string XmlFile = "config/ServerWhiteList.xml";

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static Objects.Config.WhiteList WhitelistedServers { get; set; }

        /// <summary>
        /// Loads the config
        /// </summary>
        public static void LoadConfig()
        {
            WhitelistedServers = XmlSerialization.XmlStateController.LoadObject<Objects.Config.WhiteList>(XmlFile);
        }

        /// <summary>
        /// Saves the config to the xml file
        /// </summary>
        public static void SaveConfig()
        {
            XmlSerialization.XmlStateController.SaveObject(WhitelistedServers, XmlFile);
        }
    }
}
