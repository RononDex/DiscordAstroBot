using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordAstroBot
{
    public abstract class Command
    {
        public virtual string CommandName { get; } = "";

        public virtual string[] CommandSynonyms { get; } = new string[0];

        public abstract void MessageRecieved(Match matchedMessage, MessageEventArgs e);
    }
}
