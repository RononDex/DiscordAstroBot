using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordAstroBot
{
    /// <summary>
    /// Abstract clcass for a command
    /// </summary>
    public abstract class Command
    {
        public virtual string CommandName { get; } = "";

        public virtual CommandSynonym[] CommandSynonyms { get; } = new CommandSynonym[0];

        public virtual string Description { get; } = "";

        /// <summary>
        /// Returns true when handeld, false when not
        /// </summary>
        /// <param name="matchedMessage"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public abstract Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage);
    }

    /// <summary>
    /// Describes a command synonym, to which the command will be triggered on (regex)
    /// </summary>
    public class CommandSynonym
    {
        /// <summary>
        /// A regex describing what messages the synonym will cause to trigger the command
        /// </summary>
        public string Synonym { get; set; }

        /// <summary>
        /// Command should only be triggered with a mention of the bot
        /// </summary>
        public bool OnlyWithMention { get; set; } = true;
    }
}
