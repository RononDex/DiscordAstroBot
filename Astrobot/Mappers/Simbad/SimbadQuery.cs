using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Objects.Simbad;

namespace DiscordAstroBot.Mappers.Simbad
{
    /// <summary>
    /// Allows a user to query the SIMNBAD database
    /// </summary>
    public static class SimbadQuery
    {
        /// <summary>
        /// Gets all the available data from Simbad for the given object
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static SimbadResult GetAstronomicalObjectInfo(string objectName)
        {
            var query = string.Format(SIMBADSettings.Default.ObjectQuery, objectName);

            var url = $"http://simbad.u-strasbg.fr/simbad/sim-script?script={WebUtility.UrlEncode(query)}";

            var webRequest = WebRequest.Create(url);
            string text;
            using (var response = (HttpWebResponse)webRequest.GetResponse())
            {                
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }
            }

            if (text.Contains(SIMBADSettings.Default.IdentifierNotFoundMessage))
                return null;

            var result = new SimbadResult(text);

            return result;
        }

        /// <summary>
        /// Queries for objects in a given area
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="radiusInDegrees"></param>
        /// <param name="limitingMagnitude"></param>
        /// <returns></returns>
        public static List<SimbadResult> QueryAround(RADECCoords coords, float radiusInDegrees, float limitingMagnitude)
        {
            var query = string.Format(SIMBADSettings.Default.RegionQuery, $"{coords.RA} {coords.DEC}", radiusInDegrees);

            var url = $"http://simbad.u-strasbg.fr/simbad/sim-script?script={WebUtility.UrlEncode(query)}";

            var webRequest = WebRequest.Create(url);
            string text;
            using (var response = (HttpWebResponse)webRequest.GetResponse())
                using (var sr = new StreamReader(response.GetResponseStream()))
                    text = sr.ReadToEnd();

            var splitted = text.Split(new [] { "[[end]]" }, StringSplitOptions.RemoveEmptyEntries);

            return splitted.Select(entry => new SimbadResult(entry)).ToList();
        }
    }
}
