using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using System.Reflection;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Prints out the version of the bot
    /// </summary>
    public class Version : Command
    {
        public override string CommandName => "Version";

        public override string[] CommandSynonyms => new[] {
            @"what('s|s| is)? your version(\?)?",
        };


        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            // Get last modified date of the assembly
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;

            await recievedMessage.Channel.SendMessageAsync($"{Assembly.GetExecutingAssembly().GetName().Version}, Last update: {lastModified}");
            
            return true;
        }
    }
}
