using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects
{

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
            this.ShortName = item.abbrev;
        }

        public int ID { get; set; }

        public string CountryCode { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public LaunchLibraryAgencyType Type { get; set; }

        public string InfoUrl { get; set; }

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
