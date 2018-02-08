using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.SocialMedia;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class SocialMediaTester
    {
        [TestMethod]
        public void TestInstagramPublisher()
        {
            DiscordAstroBot.Mappers.Config.ServerConfig.LoadConfig();
            DiscordAstroBot.Mappers.Config.ServerCommands.LoadConfig();
            DiscordAstroBot.Mappers.Config.SocialMediaConfig.LoadConfig();
            DiscordAstroBot.Mappers.Config.SocialMediaModQueue.LoadConfig();

            var instaProvider = new InstagramProvider();

            var postUrl = instaProvider.PublishPost(new SocialMediaPost() { Content = "", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c4/PM5544_with_non-PAL_signals.png/384px-PM5544_with_non-PAL_signals.png" });
            postUrl.Wait();
            var url = postUrl.Result;
        }
    }
}
 