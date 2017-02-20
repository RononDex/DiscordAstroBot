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
                    @"what are the upcoming launches(?'NextLaunches')(\?)?"
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
                var msg = "";
                foreach (var launch in launches)
                {
                    if (launch.TBDDate)
                    {
                        msg = $"{msg}\r\n\r\n" +
                                $"{launch.WindowStart.ToString("yyyy MMMM ")} (to be done): - {launch.Name}\r\n" +
                                $"Launching from: {launch.Location.Name} - {launch.Location.LaunchPads.FirstOrDefault()?.Name}\r\n" +
                                $"Rocket: {launch.Rocket.Name}\r\n" +
                                $"Mission Description:\r\n{launch.Missions.FirstOrDefault()?.Description}";
                    }
                    else
                    {
                        msg = $"{msg}\r\n\r\n" +
                                $"{launch.WindowStart.ToString("yyyy MMMM dd: ")} - {launch.Name}\r\n" +
                                $"Launch window: {launch.WindowStart.ToString("yyyy-MM-dd hh:mm:ss")} - {launch.WindowEnd.ToString("yyyy-MM-dd hh:mm:ss")}\r\n" +
                                $"Launching from: {launch.Location.Name} - {launch.Location.LaunchPads.FirstOrDefault()?.Name}\r\n" +
                                $"Rocket: {launch.Rocket.Name}\r\n" +
                                $"Mission Description:\r\n{launch.Missions.FirstOrDefault()?.Description}";
                    }
                }

                e.Channel.SendMessage($"These are the next upcoming launches: \r\n```\r\n{msg}\r\n```");

                return true;
            }

            return false;
        }
    }
}
