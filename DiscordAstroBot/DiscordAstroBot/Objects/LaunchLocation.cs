using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects
{
    public class LaunchLocation
    {
        public string CountryCode { get; set; }

        public int ID { get; set; }

        public string InfoUrl { get; set; }

        public string Name { get; set; }

        public List<LaunchPad> LaunchPads { get; set; } = new List<LaunchPad>();

        public string WikiURL { get; set; }

        public LaunchLocation(dynamic item)
        {
            this.CountryCode = item.countryCode;
            this.ID = item.id;
            this.InfoUrl = item.infoURL;
            this.Name = item.name;
            this.WikiURL = item.wikiURL;

            if (item.pads != null)
            {
                for (var i = 0; i < item.pads.Count; i++)
                {
                    this.LaunchPads.Add(new LaunchPad(item.pads[i]));
                }
            }
        }
    }
}
