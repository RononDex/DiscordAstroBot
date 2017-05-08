using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects.Config
{
    /// <summary>
    /// Who is the bot mad at?
    /// </summary>
    [Serializable, XmlRoot("Users"), XmlType("Users")]
    public class MadUsers
    {
        [XmlElement("User")]
        public List<MadUser> Users { get; set; } = new List<MadUser>();

        public void SaveConfig()
        {
            XmlSerialization.XmlStateController.SaveObject<MadUsers>(this, "config/MadUsers.xml");
        }
    }

    public class MadUser
    {
        /// <summary>
        /// The server on which the bot is mad at the defined user
        /// </summary>
        [XmlAttribute("Server")]
        public string Server { get; set; }

        /// <summary>
        /// The user which he is mad to
        /// </summary>
        [XmlAttribute("User")]
        public string User { get; set; }
    }
}
