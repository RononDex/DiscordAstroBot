using Discord.WebSocket;
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
        public static void EnableCommand(SocketGuild discordServer, Command command)
        {
            Log<DiscordAstroBot>.Info($"Enabling command {command.CommandName} on server {discordServer.Name}");

            // Get the server commands config
            var server = Mappers.Config.ServerCommands.Config.CommandsConfigServer.FirstOrDefault(x => x.ServerID == discordServer.Id);

            // Setup config if none exists for this server yet
            if (server == null)
            {
                server = new CommandsConfigServer() { ServerID = discordServer.Id };
                Mappers.Config.ServerCommands.SetDefaultValues(server);
                Mappers.Config.ServerCommands.Config.CommandsConfigServer.Add(server);
                Mappers.Config.ServerCommands.SaveConfig();
            }

            List<CommandConfigServerCommand> commands = server.Commands;

            if (commands.Any(x => x.CommandName == command.CommandName && x.Enabled))
                return;
            else
            {
                if (!commands.Any(x => x.CommandName == command.CommandName))
                    commands.Add(new CommandConfigServerCommand() { CommandName = command.CommandName, Enabled = true });
                else
                    commands.First(x => x.CommandName == command.CommandName).Enabled = true;

                Mappers.Config.ServerCommands.SaveConfig();
            }
        }

        /// <summary>
        /// Disables the given command on the defined server
        /// </summary>
        /// <param name="discordServer"></param>
        /// <param name="command"></param>
        public static void DisableCommand(SocketGuild discordServer, Command command)
        {
            Log<DiscordAstroBot>.Info($"Disabling command {command.CommandName} on server {discordServer.Name}");

            if (command.CommandName.ToLower() == "admincommands")
            {
                Log<DiscordAstroBot>.Info($"Tried to disable AdminCommands on server {discordServer.Name}, which is not allowed");
                return;
            }

            // Get the server commands config
            var server = Mappers.Config.ServerCommands.Config.CommandsConfigServer.FirstOrDefault(x => x.ServerID == discordServer.Id);

            // Setup config if none exists for this server yet
            if (server == null)
            {
                server = new CommandsConfigServer() { ServerID = discordServer.Id };
                Mappers.Config.ServerCommands.SetDefaultValues(server);
                Mappers.Config.ServerCommands.Config.CommandsConfigServer.Add(server);
                Mappers.Config.ServerCommands.SaveConfig();
            }

            List<CommandConfigServerCommand> commands = server.Commands;

            if (commands.Any(x => x.CommandName == command.CommandName && !x.Enabled))
                return;
            else
            {
                if (!commands.Any(x => x.CommandName == command.CommandName))
                    commands.Add(new CommandConfigServerCommand() { CommandName = command.CommandName, Enabled = false });
                else
                    commands.First(x => x.CommandName == command.CommandName).Enabled = false;

                Mappers.Config.ServerCommands.SaveConfig();
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
        public static void EnableAllCommands(SocketGuild server)
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
    }
}
