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

        /// <summary>
        /// Returns true when handeld, false when not
        /// </summary>
        /// <param name="matchedMessage"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public abstract bool MessageRecieved(Match matchedMessage, MessageEventArgs e);
    }
}
