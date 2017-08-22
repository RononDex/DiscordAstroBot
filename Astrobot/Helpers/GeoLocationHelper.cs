using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// This helper uses the Googles geolocation service to get coordinates of a given location
    /// </summary>
    public static class GeoLocationHelper
    {
        public static GeoLocation FindLocation(string address)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting GeoLocation for {0}", address);

            var webRequest = WebRequest.Create(string.Format("http://maps.googleapis.com/maps/api/geocode/json?address={0}", WebUtility.UrlEncode(address)));
            var response   = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic result = JsonConvert.DeserializeObject(text);
            if (result.results.Count == 0)
                return null;

            var hit = result.results[0];
            return new GeoLocation()
            {
                LocationName = hit.formatted_address,
                Lat          = Convert.ToSingle(hit.geometry.location.lat),
                Long         = Convert.ToSingle(hit.geometry.location.lng)
            };
        }
    }

    /// <summary>
    /// Represents a geo location
    /// </summary>
    public class GeoLocation
    {
        /// <summary>
        /// The latitude component
        /// </summary>
        public float Lat { get; set; }
        /// <summary>
        /// The longitude component
        /// </summary>
        public float Long { get; set; }

        /// <summary>
        /// Name of the address
        /// </summary>
        public string LocationName { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {2}    Lat: {0:0.00}    Lng: {1:0.00}", Lat, Long, LocationName);
        }
    }
}
