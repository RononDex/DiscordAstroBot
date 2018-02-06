using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot
{
    class Program
    {
        /// <summary>
        /// The main function (entry point for application) is basically just a wrapper to call the async Main method and wait for its execution to finish
        /// (keeps the application open)
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();            
        }

        /// <summary>
        /// Starts the bot
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            XmlConfigurator.Configure();

            Log<DiscordAstroBot>.InfoFormat("---------------------------------------------------------");
            Log<DiscordAstroBot>.InfoFormat("----------------- Launching Astro bot -------------------");
            Log<DiscordAstroBot>.InfoFormat("---------------------------------------------------------");
            Log<DiscordAstroBot>.InfoFormat("");
            Log<DiscordAstroBot>.InfoFormat("Settings:");
            Log<DiscordAstroBot>.InfoFormat(" - TokenFilePath:            {0}", ConfigurationManager.AppSettings["TokenFilePath"]);
            Log<DiscordAstroBot>.InfoFormat(" - AstrometryTokenFilePath:  {0}", ConfigurationManager.AppSettings["AstrometryTokenFilePath"]);
            Log<DiscordAstroBot>.InfoFormat($" - InstagramAPIKeyPath:      {ConfigurationManager.AppSettings["InstagramAPIKeyPath"]}");
            Log<DiscordAstroBot>.InfoFormat($" - FacebookAccessTokenPath:  {ConfigurationManager.AppSettings["FacebookAccessTokenPath"]}");
            Log<DiscordAstroBot>.InfoFormat($" - GoogleGeoLocationApiKey:  {ConfigurationManager.AppSettings["GoogleGeoLocationApiKey"]}");
            Log<DiscordAstroBot>.InfoFormat(" - ChatPrefix:               {0}", ConfigurationManager.AppSettings["ChatPrefix"]);
            Log<DiscordAstroBot>.InfoFormat(" - WhiteListEnabled:         {0}", ConfigurationManager.AppSettings["WhiteListEnabled"]);
            Log<DiscordAstroBot>.InfoFormat(" - OwnerName:                {0}", ConfigurationManager.AppSettings["OwnerName"]);
            Log<DiscordAstroBot>.InfoFormat(" - AstropyWCSConverterPath:  {0}", ConfigurationManager.AppSettings["AstropyWCSConverterPath"]);

            if (!File.Exists(ConfigurationManager.AppSettings["TokenFilePath"]))
            {
                Log<DiscordAstroBot>.FatalFormat("Could not open or access the configured token file");
                return;
            }
            if (!File.Exists(ConfigurationManager.AppSettings["AstrometryTokenFilePath"]))
            {
                Log<DiscordAstroBot>.FatalFormat("Could not open or access the configured token file");
                return;
            }

            var token = File.ReadAllText(ConfigurationManager.AppSettings["TokenFilePath"]);
            var chatPrefix = ConfigurationManager.AppSettings["ChatPrefix"];

            Log<DiscordAstroBot>.InfoFormat("Launching Astro bot...");

            var discoBot = new DiscordAstroBot();
            DiscordAstroBot.WhiteListEnabled = bool.Parse(ConfigurationManager.AppSettings["WhiteListEnabled"]);
            DiscordAstroBot.OwnerName = ConfigurationManager.AppSettings["OwnerName"];

            await discoBot.InitDiscordClient(token, chatPrefix);

            await Task.Delay(-1);
        }
    }
}
