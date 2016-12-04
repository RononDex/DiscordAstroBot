using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.Commands
{
    public class SmallTalkCommand : Command
    {

        public override string CommandName { get { return "SmallTalk"; } }

        public override void MessageRecieved(string message, MessageEventArgs e)
        {
            var reaction =  Reactions.Reactions.GetReaction(message);
            e.Channel.SendMessage(reaction);
        }
    }
}
