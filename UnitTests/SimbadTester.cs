using System;
using DiscordAstroBot.Objects.Simbad;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SimbadTester
    {
        [TestMethod]
        public void TestQueryAround()
        {
            var result = DiscordAstroBot.Mappers.Simbad.SimbadQuery.QueryAround(new RADECCoords() {DEC = "57.642", RA = "12h35min"}, 0.5f, 1000);
        }
    }
}
