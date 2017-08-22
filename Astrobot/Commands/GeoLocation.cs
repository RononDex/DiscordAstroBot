using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Command used to search for the coordinates of a given place
    /// </summary>
    public class GeoLocation : Command
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public override string CommandName => "GeoLocation";

        /// <summary>
        /// Define the synonyms for this command
        /// </summary>
        public override string[] CommandSynonyms => new string[] {
            @"What are the coordinates of (?'SearchLocation'.*\w)(\?)?",
            @"Tell me the coordinates of (?'SearchLocation'.*\w)(\?)?",
            @"Find the coordinates of (?'SearchLocation'.*\w)(\?)?",
            @"Find the coordinates from (?'SearchLocation'.*\w)(\?)?",
            @"Where is (?'SearchLocation'.*\w)(\?)?",
            @"Get the coordinates from (?'SearchLocation'.*\w)(\?)?",
            @"Get the coordinates of (?'SearchLocation'.*\w)(\?)?",
            @"Do you know the location of (?'SearchLocation'.*\w)(\?)?" };

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            // Search for the given address
            var foundLocation = Helpers.GeoLocationHelper.FindLocation(matchedMessage.Groups["SearchLocation"].Value);

            // Output result
            if (foundLocation == null)
                await recievedMessage.Channel.SendMessageAsync($"Could not find any location matching your search \"{matchedMessage.Value}\"");
            else
            {
                await recievedMessage.Channel.SendMessageAsync(foundLocation.ToString());
            }

            return true;
        }
    }
}
