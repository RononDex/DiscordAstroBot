using DiscordAstroBot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects.LaunchLibrary
{
    /// <summary>
    /// Represents a launchpad
    /// </summary>
    public class LaunchPad
    {
        /// <summary>
        /// A list of agencies that use this pad
        /// </summary>
        public List<LaunchLibraryAgency> Agencies { get; set; } = new List<LaunchLibraryAgency>();

        /// <summary>
        /// An unique ID for this launchpad
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// URL to get more info on this launchpad
        /// </summary>
        public string InfoURL { get; set; }

        /// <summary>
        /// The coordinates of this launchpad on earth (lat, long)
        /// </summary>
        public GeoLocation Coordinates { get; set; }

        /// <summary>
        /// An url to show the location of the launchpad
        /// </summary>
        public string MapURL { get; set; }

        /// <summary>
        /// The name of the launchpad
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A link to the wikipedia article of this launchpad
        /// </summary>
        public string WikiURL { get; set; }

        public LaunchPad(dynamic item)
        {
            // Initialize the object from the json object
            this.ID = item.id;
            this.InfoURL = item.infoURL;
            this.Name = item.name;
            this.Coordinates = new GeoLocation() { Lat = item.latitude, Long = item.longitude, LocationName = item.name };
            this.MapURL = item.mapURL;
            this.WikiURL = item.wikiURL;

            if (item.agencies != null)
            {
                for (var i = 0; i < item.agencies.Count; i++)
                {
                    this.Agencies.Add(new LaunchLibraryAgency(item.agencies[i]));
                }
            }
        }
    }
}
