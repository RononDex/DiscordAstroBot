using DiscordAstroBot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects
{
    public class LaunchPad
    {
        public List<LaunchLibraryAgency> Agencies { get; set; } = new List<LaunchLibraryAgency>();

        public int ID { get; set; }

        public string InfoURL { get; set; }

        public GeoLocation Coordinates { get; set; }

        public string MapURL { get; set; }

        public string Name { get; set; }

        public string WikiURL { get; set; }

        public LaunchPad(dynamic item)
        {
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
