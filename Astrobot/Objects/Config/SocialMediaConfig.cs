using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects.Config
{
    /// <summary>
    /// Value object for the social media configs
    /// </summary>
    [Serializable, XmlRoot("SocialMediaAccounts"), XmlType("SocialMediaAccounts")]
    public class SocialMediaConfig
    {
        /// <summary>
        /// List of server entries in the config file
        /// </summary>
        [XmlElement("Server")]
        public List<SocialMediaServerConfig> ServerEntries { get; set; }
    }

    /// <summary>
    /// Per server config entry
    /// </summary>
    public class SocialMediaServerConfig
    {
        /// <summary>
        /// The unique discord server ID
        /// </summary>
        [XmlAttribute("ServerID")]
        public ulong ServerID { get; set; }

        /// <summary>
        /// Social Media user settings
        /// </summary>
        [XmlElement("User")]
        public List<SocialMediaServerUserConfig> UserEntries { get; set; } = new List<SocialMediaServerUserConfig>();
    }

    /// <summary>
    /// Per user config
    /// </summary>
    public class SocialMediaServerUserConfig
    {
        /// <summary>
        /// The user of this config entry
        /// </summary>
        [XmlAttribute("ID")]
        public ulong User { get; set; }

        /// <summary>
        /// Wether the user wants his images to be published to social media
        /// If false, overwrites the per social media settings
        /// </summary>
        [XmlElement("SocialMediaPublishingEnabled")]
        public bool SocialMediaPublishingEnabled { get; set; }

        /// <summary>
        /// The facebook tag to use when posting on facebook
        /// </summary>
        [XmlElement("FacebookUserName")]
        public string FacebookUserName { get; set; }

        /// <summary>
        /// The instagram tag to use when posting on instagram
        /// </summary>
        [XmlElement("InstagramUserName")]
        public string InstagramUserName { get; set; }
    }
}
