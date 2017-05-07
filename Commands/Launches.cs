using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using DiscordAstroBot.Objects;
using DiscordAstroBot.Helpers;
using System.Globalization;

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
                    @"what are the (next|upcoming) (launches|missions) for (?'NextLaunchesQuery'(\w*\s*)*)(\?)?",
                    @"what are the (next|upcoming) (?'NextLaunchesQuery'(\w*\s*)*) (launches|missions) (\?)?",
                    @"what (launches|missions) are planned (for|by) (?'NextLaunchesQuery'(\w*\s*)*)(\?)?",
                    @"what are the upcoming launches(?'NextLaunches')(\?)?",
                    @"what is (?'AgencySearchName'.*\w)(\?)?",
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
                                $"{launch.Net.ToString("yyyy MMMM ", CultureInfo.InvariantCulture)} (to be done): {launch.Name}\r\n" +
                                $"Launching from: {launch.Location.Name} - {launch.Location.LaunchPads.FirstOrDefault()?.Name}\r\n" +
                                $"Rocket: {launch.Rocket.Name}\r\n" +
                                $"Mission Description:\r\n{launch.Missions.FirstOrDefault()?.Description}"+
                                $"\r\n--------------------------------------"; ;
                    }
                    else
                    {
                        msg = $"{msg}\r\n\r\n" +
                                $"{launch.WindowStart?.ToString("yyyy MMMM dd: ", CultureInfo.InvariantCulture)} {launch.Name}\r\n" +
                                $"Launch window (UTC): {launch.WindowStart?.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)} - {launch.WindowEnd?.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)}\r\n" +
                                $"Video: {launch.VidURL}\r\n" +
                                $"Launching from: {launch.Location.LaunchPads.FirstOrDefault()?.Name}\r\n" +
                                $"Mission Description:\r\n{launch.Missions.FirstOrDefault()?.Description}" + 
                                $"\r\n--------------------------------------";
                    }
                }

                e.Channel.SendMessage($"These are the next upcoming launches: \r\n```\r\n{msg}\r\n```");

                return true;
            }

            // When searching for upcoming launches
            if (matchedMessage.Groups["NextLaunchesQuery"] != null && matchedMessage.Groups["NextLaunchesQuery"].Success)
            {
                var launches = LaunchLibraryAPIHelper.GetNextLaunchesQuery(matchedMessage.Groups["NextLaunchesQuery"].Value);
                var msg = "";
                foreach (var launch in launches)
                {
                    if (launch.TBDDate)
                    {
                        msg = $"{msg}{launch.Net.ToString("yyyy-MM-dd ", CultureInfo.InvariantCulture)} (to be done): {launch.Name}\r\n";
                    }
                    else
                    {
                        msg = $"{msg}{launch.WindowStart?.ToString("yyyy-MM-dd ", CultureInfo.InvariantCulture)}: {launch.Name}\r\n";
                    }
                }
                e.Channel.SendMessage($"These are the upcoming launches for {matchedMessage.Groups["NextLaunchesQuery"].Value}: \r\n```\r\n{msg}\r\n```");
                return true;
            }

            return false;
        }
    }
}
