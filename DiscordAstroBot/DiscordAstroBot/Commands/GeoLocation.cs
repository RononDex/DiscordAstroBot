using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;

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
        public override string CommandName { get { return "GeoLocation"; } }

        /// <summary>
        /// Define the synonyms for this command
        /// </summary>
        public override string[] CommandSynonyms
        {
            get
            {
                return new string[] {
                    "What are the coordinates of (?'SearchLocation'.*)",
                    "Tell me the coordinates of (?'SearchLocation'.*)",
                    "Find the coordinates of (?'SearchLocation'.*)",
                    "Find the coordinates from (?'SearchLocation'.*)",
                    "Where is (?'SearchLocation'.*)",
                    "Get the coordinates from (?'SearchLocation'.*)",
                    "Get the coordinates of (?'SearchLocation'.*)"};
            }
        }

        public override void MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            // Search for the given address
            var foundLocation = Helpers.GeoLocationHelper.FindLocation(matchedMessage.Groups["SearchLocation"].Value);

            // Output result
            if (foundLocation == null)
                e.Channel.SendMessage(string.Format("Could not find any location matching your search \"{0}\"", matchedMessage.Value));
            else
            {
                e.Channel.SendMessage(foundLocation.ToString());
            }
        }
    }
}
