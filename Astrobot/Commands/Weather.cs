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
    public class Weather : Command
    {
        public override string[] CommandSynonyms
        {
            get
            {
                return new string[] {
                    @"(whats|what's|show\sme|how is|hows|how's|what is) the weather (like )?(in|for) (?'SearchLocation'.*\w)(\?)?",
                };
            }
        }

        public override string CommandName { get { return "Weather"; } }

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            var location = Helpers.GeoLocationHelper.FindLocation(matchedMessage.Groups["SearchLocation"].Value);
            // If no loacation with that name found, stop searching for a weather forcast
            if (location == null)
            {
                await recievedMessage.Channel.SendMessageAsync(string.Format("I don't know any place on this planet that is called \"{0}\"", matchedMessage.Groups["SearchLocation"].Value));
                return true;
            }

            await recievedMessage.Channel.SendMessageAsync(string.Format("Hold on, searching a weather forcast for {0}. This might take a moment...", location));
            var forcast = Helpers.WeatherHelper.GetWeatherForcast(DateTime.Today, location);
            await recievedMessage.Channel.SendMessageAsync(string.Format("This is the weather forcast that I found for {0}:", matchedMessage.Groups["SearchLocation"].Value));
            await recievedMessage.Channel.SendFileAsync(new MemoryStream(forcast.Screenshot), "Weather_forcast.png" );

            return true;
        }
    }
}
