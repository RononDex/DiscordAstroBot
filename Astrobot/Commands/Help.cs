using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Command to display help on how to use the bot
    /// </summary>
    public class Help : Command
    {
        public override string CommandName => "Help";

        public override string[] CommandSynonyms => new [] { "help" };

        public override string Description => "Displays this help";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            var output = string.Join("\r\n",
                Mappers.Config.ServerCommands.Config.CommandsConfigServer.Single(
                    x => x.ServerID == ((SocketGuildChannel) recievedMessage.Channel).Guild.Id)
                    .Commands.Where(x => x.Enabled)
                    .Select(x => $"**{x.CommandName}**: {Utilities.CommandsUtility.ResolveCommand(x.CommandName).Description}"));
            await recievedMessage.Channel.SendMessageAsync($"Available commands on this server:\r\n{output}");
            return true;
        }
    }
}
