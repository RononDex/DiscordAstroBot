using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects
{

    /// <summary>
    /// Represents a space agency
    /// </summary>
    public class LaunchLibraryAgency
    {
        public LaunchLibraryAgency(dynamic item)
        {
            // Initialize the object from the json object
            this.CountryCode = item.countryCode;
            this.ID = item.id;
            this.InfoUrl = item.infoURL;
            this.Name = item.name;
            this.Type = (LaunchLibraryAgencyType)item.type;
            this.WikiUrl = item.wikiURL;
            this.ShortName = item.abbrev;
        }

        /// <summary>
        /// A unique ID for the space agency
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The associated country with this agency
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// A short name (Abbreviation)
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The agencies name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the agency
        /// </summary>
        public LaunchLibraryAgencyType Type { get; set; }

        /// <summary>
        /// webpage of the agency
        /// </summary>
        public string InfoUrl { get; set; }

        /// <summary>
        /// Wikipedia article to this agency
        /// </summary>
        public string WikiUrl { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} ({ShortName})\r\n" +
                   $"Type: {Type}\r\n" +
                   $"Country: {CountryCode}\r\n" +
                   $"InfoUrl: {InfoUrl}\r\n" +
                   $"WikuUrl: {WikiUrl}";
        }
    }

    public enum LaunchLibraryAgencyType
    {
        GOVERMENT = 1,
        MULTINATIONAL = 2,
        COMMERCIAL = 3,
        EDUCATIONAL = 4,
        PRIVATE = 5,
        UNKNOWN = 6
    }
}
