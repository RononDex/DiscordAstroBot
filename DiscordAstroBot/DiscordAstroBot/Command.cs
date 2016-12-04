using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot
{
    abstract class Command
    {
        public virtual string CommandName { get; set; }

        public abstract void MessageRecieved(string message, MessageEventArgs e);
    }
}
