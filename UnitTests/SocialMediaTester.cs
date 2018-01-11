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
            var instaProvider = new InstagramProvider();

            var postUrl = instaProvider.PublishPost(new SocialMediaPost() { Content = "", Image = File.ReadAllBytes("TestData/testimage.jpg") });
        }
    }
}
 