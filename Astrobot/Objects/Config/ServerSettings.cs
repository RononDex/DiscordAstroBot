using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects.Config
{
    /// <summary>
    /// Per server config settings
    /// </summary>
    [Serializable, XmlRoot("ServerConfig"), XmlType("ServerConfig")]
    public class ServerSettings
    {
        /// <summary>
        /// Gets a list of all per server configs
        /// </summary>
        [XmlElement("Server")]
        public List<ServerSettingsServer> Servers { get; set; } = new List<ServerSettingsServer>();
    }

    /// <summary>
    /// Config for a single server
    /// </summary>
    public class ServerSettingsServer
    {
        /// <summary>
        /// The unique discord server ID
        /// </summary>
        [XmlAttribute("ID")]
        public ulong ServerID { get; set; }

        /// <summary>
        /// Gets the list of all config values
        /// </summary>
        [XmlElement("Config")]
        public List<ServerSettingsServerConfig> Configs { get; set; } = new List<ServerSettingsServerConfig>();
    }

    /// <summary>
    /// A single config entry
    /// </summary>
    public class ServerSettingsServerConfig
    {
        /// <summary>
        /// The unique name of this config
        /// </summary>
        [XmlAttribute("Key")]
        public string Key { get; set; }

        /// <summary>
        /// The value this config-key is set to
        /// </summary>
        [XmlAttribute("Value")]
        public string Value { get; set; }
    }
}
