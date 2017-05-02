using DiscordAstroBot.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Utilities
{
    public static class CommandsUtil
    {
        public static void EnableCommand(Discord.Server discordServer, string command)
        {
            var server = Config.CommandsConfig.CommandsConfigServer.FirstOrDefault(x => x.ServerID == discordServer.Id);
            List<CommandConfigServerCommand> commands = new List<CommandConfigServerCommand>();
            if (server != null)
            {
                commands = server.Commands;
            }
            else
            {
                Config.CommandsConfig.CommandsConfigServer.Add(new CommandsConfigServer() { ServerID = discordServer.Id, Commands = commands });
                Config.CommandsConfig.SaveConfig();
            }

            if (commands.Any(x => x.CommandName.ToLower() == command.ToLower() && x.Enabled))
                return;
            else
            {
                if (!commands.Any(x => x.CommandName.ToLower() == command.ToLower()))
                    commands.Add(new CommandConfigServerCommand() { CommandName = command.ToLower(), Enabled = true });
                else
                    commands.First(x => x.CommandName.ToLower() == command.ToLower()).Enabled = true;

                Config.CommandsConfig.SaveConfig();
            }
        }
    }
}
