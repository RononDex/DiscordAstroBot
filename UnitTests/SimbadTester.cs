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
            var result = DiscordAstroBot.Mappers.Simbad.SimbadQuery.QueryAround(new RADECCoords() {DEC = 47.56332f, RA = 13.654645f }, 0.5f);
        }
    }
}
