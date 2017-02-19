using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Text.RegularExpressions;

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

        public override bool MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            var location = Helpers.GeoLocationHelper.FindLocation(matchedMessage.Groups["SearchLocation"].Value);
            // If no loacation with that name found, stop searching for a weather forcast
            if (location == null)
            {
                e.Channel.SendMessage(string.Format("I don't know any place on this planet that is called \"{0}\"", matchedMessage.Groups["SearchLocation"].Value));
                return true;
            }

            e.Channel.SendMessage(string.Format("Hold on, searching a weather forcast for {0}. This might take a moment...", location));
            var forcast = Helpers.WeatherHelper.GetWeatherForcast(DateTime.Today, location);
            e.Channel.SendMessage(string.Format("This is the weather forcast that I found for {0}:", matchedMessage.Groups["SearchLocation"].Value));
            e.Channel.SendFile("Weather_forcast.png", new MemoryStream(forcast.Screenshot));

            return true;
        }
    }
}
