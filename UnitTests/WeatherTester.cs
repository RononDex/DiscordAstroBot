using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class WeatherTester
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestCreateWeatherForcast()
        {
            var location = DiscordAstroBot.Helpers.GeoLocationHelper.FindLocation("zurich");
        }
    }
}
