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
    public class Help : Command
    {
        public override string CommandName => "Help";

        public override string[] CommandSynonyms => base.CommandSynonyms;

        public override string Description => "Displays this help";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            return false;
        }
    }
}
