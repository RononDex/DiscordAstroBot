using DiscordAstroBot.Objects;
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
        /// <summary>
        /// Gets all the agencies, that match the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<LaunchLibraryAgency> FindAgenciesByName(string name)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting agencies by name {0}", name);

            var webRequest = WebRequest.CreateHttp(string.Format("http://launchlibrary.net/1.2/agency?name={0}&mode=verbose", WebUtility.UrlEncode(name)));
            webRequest.Accept = "application/json";
            webRequest.Headers["X-Requested-With"] = "DiscordAstroBot";
            webRequest.UserAgent = "DiscordAstroBot";

            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic result = JsonConvert.DeserializeObject(text);
            if (result.agencies.Count == 0)
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

        /// <summary>
        /// Gets only one agency, matching the name, priorising short name matches
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static LaunchLibraryAgency GetSpaceAgency(string name)
        {
            var agencies = Helpers.LaunchLibraryAPIHelper.FindAgenciesByName(name);
            var byShortName = Helpers.LaunchLibraryAPIHelper.FindByShortName(name);

            LaunchLibraryAgency agency = null;

            if (byShortName != null)
                agency = byShortName;
            else if (agencies != null && agencies.Count > 0)
                agency = agencies.First();

            return agency;
        }

        /// <summary>
        /// Finds a agency by its short name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static LaunchLibraryAgency FindByShortName(string name)
        {

            Log<DiscordAstroBot>.InfoFormat("Requesting agencies by shortname {0}", name);

            var webRequest = WebRequest.CreateHttp(string.Format("http://launchlibrary.net/1.2/agency/{0}", WebUtility.UrlEncode(name)));
            webRequest.Accept = "application/json";
            webRequest.Headers["X-Requested-With"] = "DiscordAstroBot";
            webRequest.UserAgent = "DiscordAstroBot";

            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic result = JsonConvert.DeserializeObject(text);

            if (result.agencies.Count == 0)
                return null;

            if (result.agencies == null || result.agencies.Count == 0)
                return null;


            return new LaunchLibraryAgency(result.agencies[0]);
        }

        public static List<SpaceLaunch> GetNextLaunches()
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting the next upcoming launches");

            var webRequest = WebRequest.CreateHttp("http://launchlibrary.net/1.2/launch/next/3");
            webRequest.Accept = "application/json";
            webRequest.Headers["X-Requested-With"] = "DiscordAstroBot";
            webRequest.UserAgent = "DiscordAstroBot";

            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic result = JsonConvert.DeserializeObject(text);
            if (result.launches.Count == 0)
                return null;

            if (result.launches == null || result.launches.Count == 0)
                return null;

            var launches = new List<SpaceLaunch>();

            for (var i = 0; i < result.launches.Count; i++)
            {
                launches.Add(new SpaceLaunch(result.launches[i]));
            }

            return launches;
        }

        public static List<SpaceLaunch> GetNextLaunchesQuery(string query)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting the next upcoming launches");

            var webRequest = WebRequest.CreateHttp($"http://launchlibrary.net/1.2/launch?name={query}&startdate={DateTime.Today.ToString("yyyy-MM-dd")}");
            webRequest.Accept = "application/json";
            webRequest.Headers["X-Requested-With"] = "DiscordAstroBot";
            webRequest.UserAgent = "DiscordAstroBot";

            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic result = JsonConvert.DeserializeObject(text);
            if (result.launches.Count == 0)
                return null;

            if (result.launches == null || result.launches.Count == 0)
                return null;

            var launches = new List<SpaceLaunch>();

            for (var i = 0; i < result.launches.Count; i++)
            {
                var launch = new SpaceLaunch(result.launches[i]);
                launches.Add(launch);
            }

            return launches;
        }
    }
}
