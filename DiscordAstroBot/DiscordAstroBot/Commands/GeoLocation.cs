using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

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
                    "What are the coordinates of",
                    "Tell me the coordinates of",
                    "Find the coordinates of",
                    "Find the coordinates from",
                    "Where is",
                    "Get the coordinates from",
                    "Get the coordinates of"};
            }
        }

        public override void MessageRecieved(string message, MessageEventArgs e)
        {
            // Search for the given address
            var foundLocation = Helpers.GeoLocationHelper.FindLocation(message);

            // Output result
            if (foundLocation == null)
                e.Channel.SendMessage(string.Format("Could not find any location matching your search \"{0}\"", message));
            else
            {
                e.Channel.SendMessage(foundLocation.ToString());
            }
        }
    }
}
