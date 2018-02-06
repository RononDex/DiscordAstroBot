using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects.Config
{
    /// <summary>
    /// Serializable object that contains a list of whitelisted servers
    /// </summary>
    [Serializable, XmlRoot("ServerWhiteList"), XmlType("ServerWhiteList")]
    public class WhiteList
    {
        /// <summary>
        /// The list of all whitelisted servers
        /// </summary>
        [XmlElement("Entry")]
        public List<WhiteListEntry> Entries { get; set; } = new List<WhiteListEntry>();
    }

    public class WhiteListEntry
    {
        /// <summary>
        /// The unique discord server ID
        /// </summary>
        [XmlAttribute("ServerID")]
        public ulong ServerID { get; set; }

        /// <summary>
        /// Wether social media posting is allowed from this server or not
        /// </summary>
        [XmlAttribute("SocialMediaEnabled")]
        public bool SocialMediaEnabled { get; set; }
    }
}
