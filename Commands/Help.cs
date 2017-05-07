using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.Commands
{
    public class Help : Command
    {
        public override string CommandName => "Help";

        public override string[] CommandSynonyms => base.CommandSynonyms;

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            return false;
        }
    }
}
