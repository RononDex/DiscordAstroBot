using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    public class TestCommand : Command
    {
        public override string CommandName => "test";

        public override string[] CommandSynonyms => new[] { "test" };

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            await recievedMessage.Channel.SendMessageAsync($"IT'S WORKING!!! You entered: {matchedMessage.Value}");

            return true;
        }
    }
}
