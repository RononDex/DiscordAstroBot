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
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Log<DiscordAstroBot>.InfoFormat("---------------------------------------------------------");
            Log<DiscordAstroBot>.InfoFormat("---------------- Launching up Astro bot -----------------");
            Log<DiscordAstroBot>.InfoFormat("---------------------------------------------------------");
            Log<DiscordAstroBot>.InfoFormat("");
            Log<DiscordAstroBot>.InfoFormat("Settings:");
            Log<DiscordAstroBot>.InfoFormat(" - TokenFilePath: {0}", ConfigurationManager.AppSettings["TokenFilePath"]);
            Log<DiscordAstroBot>.InfoFormat(" - AstrometryTokenFilePath: {0}", ConfigurationManager.AppSettings["AstrometryTokenFilePath"]);
            Log<DiscordAstroBot>.InfoFormat(" - ChatPrefix: {0}", ConfigurationManager.AppSettings["ChatPrefix"]);

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
            var discoBot = new DiscordAstroBot(token, chatPrefix);
        }
    }
}
