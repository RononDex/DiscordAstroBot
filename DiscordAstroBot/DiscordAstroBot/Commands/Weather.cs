using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;

namespace DiscordAstroBot.Commands
{
    public class Weather : Command
    {
        public override string[] CommandSynonyms
        {
            get
            {
                return new string[] {
                    "Show me the weather for",
                    "Show me the weather forcast for",
                    "whats the weather in",
                    "whats the weather like in",
                    "what's the weather in",
                    "what's the weather like in",
                    "how is the weather in",
                    "how is the weather like in",
                    "how's the weather in",
                    "how's the weather like in",
                    "hows the weather in",
                    "hows the weather like in",
                    "how does the weather look like in",
                };
            }
        }

        public override string CommandName { get { return "Weather"; } }

        public override void MessageRecieved(string message, MessageEventArgs e)
        {
            var location = Helpers.GeoLocationHelper.FindLocation(message);
            e.Channel.SendMessage(string.Format("Hold on, searching a weather forcast for {0}. This might take a moment...", location));
            var forcast = Helpers.WeatherHelper.GetWeatherForcast(DateTime.Today, location);

            e.Channel.SendMessage(string.Format("This is the weather forcast that I found for {0}:", message));
            e.Channel.SendFile("Weather_forcast.png", new MemoryStream(forcast.Screenshot));
        }
    }
}
