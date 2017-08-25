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
            var result = DiscordAstroBot.Mappers.Simbad.SimbadQuery.QueryAround(new RADECCoords() {DEC = "+47 11 42.93", RA = "13 29 52.698" }, 0.5f, 1000);
        }
    }
}
