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
    public static class ServerCommands
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        public const string XmlFile = "config/Commands.xml";

        /// <summary>
        /// The default per server config
        /// </summary>
        public static List<CommandConfigServerCommand> DefaultConfig => new List<CommandConfigServerCommand>()
        {
            new CommandConfigServerCommand() {CommandName = "AdminCommands", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "AstroMetry", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "GeoLocation", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "Help", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "Launches", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "Simbad", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "SmallTalk", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "test", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "Version", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "Weather", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "DSS", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "UserCommands", Enabled = true},
            new CommandConfigServerCommand() {CommandName = "SocialMedia", Enabled = true},
        };

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static CommandsConfig Config { get; set; }

        /// <summary>
        /// Loads the config
        /// </summary>
        public static void LoadConfig()
        {
            Config = XmlSerialization.XmlStateController.LoadObject<CommandsConfig>(XmlFile);

            // Default values
            foreach (var server in Config.CommandsConfigServer)
                SetDefaultValues(server);
        }

        /// <summary>
        /// Sets the default values on a given server
        /// </summary>
        /// <param name="serverCfg"></param>
        public static void SetDefaultValues(CommandsConfigServer serverCfg)
        {
            foreach (var defaultValue in DefaultConfig)
            {
                if (!serverCfg.Commands.Any(x => x.CommandName.ToLower() == defaultValue.CommandName.ToLower()))
                    serverCfg.Commands.Add(defaultValue);
            }
        }

        /// <summary>
        /// Saves the config to the xml file
        /// </summary>
        public static void SaveConfig()
        {
            XmlSerialization.XmlStateController.SaveObject<CommandsConfig>(Config, XmlFile);
        }
    }
}
