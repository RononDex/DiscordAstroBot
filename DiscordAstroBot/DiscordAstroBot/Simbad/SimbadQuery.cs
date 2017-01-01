using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Simbad
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

            var url = string.Format("http://simbad.u-strasbg.fr/simbad/sim-script?script={0}", WebUtility.UrlEncode(query));

            var webRequest = WebRequest.Create(url);
            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            if (text.Contains(SIMBADSettings.Default.IdentifierNotFoundMessage))
                return null;

            var result = new SimbadResult(text);

            return result;
        }
    }
}
