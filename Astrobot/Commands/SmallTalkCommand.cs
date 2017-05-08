using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;
using DiscordAstroBot.Helpers;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Command to do smalltalk
    /// </summary>
    public class SmallTalkCommand : Command
    {

        public override string CommandName { get { return "SmallTalk"; } }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            var reaction =  ReactionsHelper.GetReaction(matchedMessage.Value.ToLower(), e);
            e.Channel.SendMessage(reaction);

            return true;
        }
    }
}
