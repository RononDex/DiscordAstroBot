using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Objects.Config;

namespace DiscordAstroBot.Mappers.Config
{
    public class TimerJobExecutions
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        public const string XmlFile = "config/TimerJobExecutions.xml";

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static Objects.Config.TimerJobExecutions Config { get; set; }

        /// <summary>
        /// Loads the config
        /// </summary>
        public static void LoadConfig()
        {
            Config = XmlSerialization.XmlStateController.LoadObject<Objects.Config.TimerJobExecutions>(XmlFile);
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
