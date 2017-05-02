﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects
{
    [Serializable, XmlRoot("Commands"), XmlType("Commands")]
    public class CommandsConfig
    {
        [XmlElement("Server")]
        public List<CommandsConfigServer> CommandsConfigServer { get; set; }

        public void SaveConfig()
        {
            XmlSerialization.XmlStateController.SaveObject<CommandsConfig>(this, "config/Commands.xml");
        }
    }

    public class CommandsConfigServer
    {
        /// <summary>
        /// The id of the server
        /// </summary>
        [XmlAttribute("ID")]
        public ulong ServerID { get; set; }

        [XmlElement("Command")]
        public List<CommandConfigServerCommand> Commands { get; set; }
    }

    public class CommandConfigServerCommand
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        [XmlAttribute("Name")]
        public string CommandName { get; set; }

        /// <summary>
        /// True if this command is enabled on the current server
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
    }

}