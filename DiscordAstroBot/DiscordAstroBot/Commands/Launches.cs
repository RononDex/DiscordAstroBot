using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using DiscordAstroBot.Objects;
using DiscordAstroBot.Helpers;

namespace DiscordAstroBot.Commands
{
    class Launches : Command
    {
        public override string CommandName { get { return "Launches"; } }

        public override string[] CommandSynonyms
        {
            get
            {
                return new string[] {
                    @"what do you know about (?'AgencySearchName'.*\w)(\?)?",
                    @"what is (?'AgencySearchName'.*\w)(\?)?",
                    @"what are the upcoming launches (?'NextLaunches'.*\w)(\?)?"
                };
            }
        }

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            // When searching for a specific agency
            if (matchedMessage.Groups["AgencySearchName"] != null && matchedMessage.Groups["AgencySearchName"].Success)
            {
                var agency = LaunchLibraryAPIHelper.GetSpaceAgency(matchedMessage.Groups["AgencySearchName"].Value);

                if (agency == null)
                    return false;

                e.Channel.SendMessage($"I found the following space agency matching your search:\r\n```\r\n" +
                                      $"{agency}\r\n" +
                                      $"```");

                return true;
            }

            // When searching for launches
            if (matchedMessage.Groups["NextLaunches"] != null && matchedMessage.Groups["NextLaunches"].Success)
            {
                var launches = LaunchLibraryAPIHelper.GetNextLaunches();

                return true;
            }

            return false;
        }
    }
}
