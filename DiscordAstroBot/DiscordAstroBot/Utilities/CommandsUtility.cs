using DiscordAstroBot.Objects.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Utilities
{
    public static class CommandsUtility
    {
        /// <summary>
        /// Enables a given command on the given server
        /// </summary>
        /// <param name="discordServer"></param>
        /// <param name="command"></param>
        public static void EnableCommand(Discord.Server discordServer, Command command)
        {
            Log<DiscordAstroBot>.Info($"Enabling command {command.CommandName} on server {discordServer.Name}");

            // Get the server commands config
            var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == discordServer.Id);

            // Setup config if none exists for this server yet
            if (server == null)
                server = SetupServerConfig(discordServer);

            List<CommandConfigServerCommand> commands = server.Commands;

            if (commands.Any(x => x.CommandName == command.CommandName && x.Enabled))
                return;
            else
            {
                if (!commands.Any(x => x.CommandName == command.CommandName))
                    commands.Add(new CommandConfigServerCommand() { CommandName = command.CommandName, Enabled = true });
                else
                    commands.First(x => x.CommandName == command.CommandName).Enabled = true;

                Config.CommandsConfig.SaveConfig();
            }
        }

        /// <summary>
        /// Disables the given command on the defined server
        /// </summary>
        /// <param name="discordServer"></param>
        /// <param name="command"></param>
        public static void DisableCommand(Discord.Server discordServer, Command command)
        {
            Log<DiscordAstroBot>.Info($"Disabling command {command.CommandName} on server {discordServer.Name}");

            if (command.CommandName.ToLower() == "admincommands")
            {
                Log<DiscordAstroBot>.Info($"Tried to disable AdminCommands on server {discordServer.Name}, which is not allowed");
                return;
            }

            // Get the server commands config
            var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == discordServer.Id);

            // Setup config if none exists for this server yet
            if (server == null)
                server = SetupServerConfig(discordServer);

            List<CommandConfigServerCommand> commands = server.Commands;

            if (commands.Any(x => x.CommandName == command.CommandName && !x.Enabled))
                return;
            else
            {
                if (!commands.Any(x => x.CommandName == command.CommandName))
                    commands.Add(new CommandConfigServerCommand() { CommandName = command.CommandName, Enabled = false });
                else
                    commands.First(x => x.CommandName == command.CommandName).Enabled = false;

                Config.CommandsConfig.SaveConfig();
            }
        }

        /// <summary>
        /// Tries to resolve the command name to a physical command instance
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static Command ResolveCommand(string command)
        {
            return DiscordAstroBot.Commands.FirstOrDefault(x => x.CommandName.ToLower() == command.ToLower());
        }

        /// <summary>
        /// Enables all the commands on the given server
        /// </summary>
        /// <param name="server"></param>
        public static void EnableAllCommands(Discord.Server server)
        {
            Log<DiscordAstroBot>.InfoFormat($"Enabling all commands on server {server.Name}");

            foreach (var command in GetAllRegisteredCommands())
            {
                EnableCommand(server, command);
            }
        }

        /// <summary>
        /// Gets a list of all registered commands on the bot
        /// </summary>
        /// <returns></returns>
        public static List<Command> GetAllRegisteredCommands()
        {
            return DiscordAstroBot.Commands.ToList();
        }

        /// <summary>
        /// Creates a new server commands config with default commands enabled
        /// </summary>
        /// <returns></returns>
        public static CommandsConfigServer SetupServerConfig(Discord.Server server)
        {
            Log<DiscordAstroBot>.Info($"Setting up commands config for server {server.Name}");

            var serverCfg = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == server.Id);

            // Check if config already exists
            if (serverCfg != null)
                return serverCfg;

            serverCfg = new CommandsConfigServer() { ServerID = server.Id, Commands = new List<CommandConfigServerCommand>() };

            // Create server config
            Config.CommandsConfig.CommandsConfigServer.Add(serverCfg);

            // Enable default commands
            EnableCommand(server, ResolveCommand("AdminCommands"));

            // Save config
            Config.CommandsConfig.SaveConfig();

            return serverCfg;
        }
    }
}
