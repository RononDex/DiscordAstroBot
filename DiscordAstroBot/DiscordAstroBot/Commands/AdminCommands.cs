using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Controls at who the bot is mad at
    /// </summary>
    public class AdminCommands : Command
    {
        public override string[] CommandSynonyms { get; }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
