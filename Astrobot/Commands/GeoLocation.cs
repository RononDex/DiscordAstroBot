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
                    @"What are the coordinates of (?'SearchLocation'.*\w)(\?)?",
                    @"Tell me the coordinates of (?'SearchLocation'.*\w)(\?)?",
                    @"Find the coordinates of (?'SearchLocation'.*\w)(\?)?",
                    @"Find the coordinates from (?'SearchLocation'.*\w)(\?)?",
                    @"Where is (?'SearchLocation'.*\w)(\?)?",
                    @"Get the coordinates from (?'SearchLocation'.*\w)(\?)?",
                    @"Get the coordinates of (?'SearchLocation'.*\w)(\?)?",
                    @"Do you know the location of (?'SearchLocation'.*\w)(\?)?" };
            }
        }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
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

            return true;
        }
    }
}
