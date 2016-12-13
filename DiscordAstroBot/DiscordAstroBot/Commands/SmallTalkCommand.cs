using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;

namespace DiscordAstroBot.Commands
{
    public class SmallTalkCommand : Command
    {

        public override string CommandName { get { return "SmallTalk"; } }

        public override void MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            var reaction =  Reactions.Reactions.GetReaction(matchedMessage.Value.ToLower());
            e.Channel.SendMessage(reaction);
        }
    }
}
