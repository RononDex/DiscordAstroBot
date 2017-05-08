using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects.LaunchLibrary
{
    /// <summary>
    /// Represents a location, where space launches are started from
    /// </summary>
    public class LaunchLocation
    {
        /// <summary>
        /// The country in which this launch complex is located at
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// An unique ID for this Launch Location
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// An URL to a webpage, with further information on this location
        /// </summary>
        public string InfoUrl { get; set; }

        /// <summary>
        /// The name of this location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of launchpads located within this complex
        /// </summary>
        public List<LaunchPad> LaunchPads { get; set; } = new List<LaunchPad>();

        /// <summary>
        /// A link to a Wikipedia article with further information
        /// </summary>
        public string WikiURL { get; set; }
        
        public LaunchLocation(dynamic item)
        {
            // Initialize the object from the json object
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
