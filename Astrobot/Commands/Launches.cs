﻿using System.Linq;
using System.Text.RegularExpressions;
using Discord;

using System.Globalization;
using DiscordAstroBot.Mappers.LaunchLibrary;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Command to query upcoming rocket launches and anything related to launches
    /// </summary>
    class Launches : Command
    {
        public override string CommandName => "Launches";

        public override CommandSynonym[] CommandSynonyms => new [] {
            new CommandSynonym() { Synonym = @"what do you know about (?'AgencySearchName'.*\w)(\?)?" },
            new CommandSynonym() { Synonym = @"what are the (next|upcoming) (launches|missions) for (?'NextLaunchesQuery'(\w*\s*)*)(\?)?" },
            new CommandSynonym() { Synonym = @"what are the (next|upcoming) (?'NextLaunchesQuery'(\w*\s*)*) (launches|missions) (\?)?" },
            new CommandSynonym() { Synonym = @"what (launches|missions) are planned (for|by) (?'NextLaunchesQuery'(\w*\s*)*)(\?)?" },
            new CommandSynonym() { Synonym = @"what are the upcoming launches(?'NextLaunches')(\?)?" },
            new CommandSynonym() { Synonym = @"what is (?'AgencySearchName'.*\w)(\?)?" },
        };

        public override string Description => "Can give info on upcoming rocket launches. Usage:\r\n```    @Astro Bot What are the upcoming launches?```";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            // When searching for a specific agency
            if (matchedMessage.Groups["AgencySearchName"] != null && matchedMessage.Groups["AgencySearchName"].Success)
            {
                var agency = LaunchLibraryAPIHelper.GetSpaceAgency(matchedMessage.Groups["AgencySearchName"].Value);

                if (agency == null)
                    return false;

                await recievedMessage.Channel.SendMessageAsync($"I found the following space agency matching your search:\r\n```\r\n" +
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

                await recievedMessage.Channel.SendMessageAsync($"These are the next upcoming launches: \r\n```\r\n{msg}\r\n```");

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
                await recievedMessage.Channel.SendMessageAsync($"These are the upcoming launches for {matchedMessage.Groups["NextLaunchesQuery"].Value}: \r\n```\r\n{msg}\r\n```");
                return true;
            }

            return false;
        }
    }
}
