using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Provides weather forcasts for astronomy
    /// </summary>
    public class Weather : Command
    {
        public override string[] CommandSynonyms => new string[] {
            @"(whats|what's|show\sme|how is|hows|how's|what is) the (weather|forcast) (like )?(in|for) (?'SearchLocation'.*\w)(\?)?",
            @"(weather|forcast) (in|for) (?'SearchLocation'.*\w)(\?)?",
            @"clearoutside (?'SearchLocationCL'.*\w)(\?)?"
        };

        public override string CommandName => "Weather";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            Helpers.GeoLocation location;

            if (matchedMessage.Groups["SearchLocationCL"].Success == false)
                location = Helpers.GeoLocationHelper.FindLocation(matchedMessage.Groups["SearchLocation"].Value);
            else
            {
                location =  Helpers.GeoLocationHelper.FindLocation(matchedMessage.Groups["SearchLocationCL"].Value);
            }
            // If no loacation with that name found, stop searching for a weather forcast
            if (location == null)
            {
                await recievedMessage.Channel.SendMessageAsync($"I don't know any place on this planet that is called \"{matchedMessage.Groups["SearchLocation"].Value}\"");
                return true;
            }

            await recievedMessage.Channel.SendMessageAsync($"Hold on, searching a weather forcast for {location}. This might take a moment...");
            Helpers.WeatherForcast forcast;
            if (matchedMessage.Groups["SearchLocation"].Success)
                forcast = Helpers.WeatherHelper.GetWeatherForcastMeteoBlue(location);
            else
                forcast = Helpers.WeatherHelper.GetWeatherForcastClearOutside(location);

            await recievedMessage.Channel.SendMessageAsync($"This is the weather forcast that I found for {matchedMessage.Groups["SearchLocation"].Value}:");
            await recievedMessage.Channel.SendFileAsync(new MemoryStream(forcast.Screenshot), "Weather_forcast.png" );

            return true;
        }
    }
}
