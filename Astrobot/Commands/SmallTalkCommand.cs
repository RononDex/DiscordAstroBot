using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;
using DiscordAstroBot.Helpers;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Command to do smalltalk
    /// </summary>
    public class SmallTalkCommand : Command
    {

        public override string CommandName => "SmallTalk";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            var reaction =  ReactionsHelper.GetReaction(matchedMessage.Value.ToLower(), recievedMessage);
            await recievedMessage.Channel.SendMessageAsync(reaction);

            return true;
        }
    }
}
