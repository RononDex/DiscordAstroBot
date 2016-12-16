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
    public static class LaunchLibraryAPIHelper
    {
        public static List<LaunchLibraryAgency> FindAgenciesByName(string name)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting agencies by name {0}", name);

            var webRequest = WebRequest.CreateHttp(string.Format("http://launchlibrary.net/1.2/agency?name={0}&mode=verbose", WebUtility.UrlEncode(name)));
            webRequest.Accept = "application/json";
            //webRequest.ContentType = "application/json";
            webRequest.Headers["X-Requested-With"] = "DiscordAstroBot";
            //webRequest.KeepAlive = true;
            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic result = JsonConvert.DeserializeObject(text);
            if (result.results.Count == 0)
                return null;
                
            if (result.agencies == null || result.agencies.Count == 0)
                return null;

            var agencies = new List<LaunchLibraryAgency>();

            for (var i = 0; i < result.agencies.Count; i++)
            {
                agencies.Add(new LaunchLibraryAgency(result.agencies[i]));
            }

            return agencies;
        }
    }

    public class LaunchLibraryAgency 
    {
        public LaunchLibraryAgency() { }
        
        public LaunchLibraryAgency(dynamic item)
        {
            this.CountryCode = item.countryCode;
            this.ID = item.id;
            this.InfoUrl = item.infoURL;
            this.Name = item.name;
            this.Type = (LaunchLibraryAgencyType)item.type;
            this.WikiUrl = item.wikiURL;
        }

        public int ID { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public LaunchLibraryAgencyType Type { get; set; }

        public string InfoUrl { get; set; }

        public string WikiUrl { get; set; }
    }

    public enum LaunchLibraryAgencyType
    {
        GOVERMENT = 1,
        MULTINATIONAL = 2,
        COMMERSCIAL = 3,
        EDUCATIONAL = 4,
        PRIVATE = 5,
        UNKNOWN = 6
    }
}
