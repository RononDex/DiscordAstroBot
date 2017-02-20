using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Util.TypeConverters;

namespace DiscordAstroBot.Objects
{
    public class Rocket
    {
        public List<LaunchLibraryAgency> Agencies { get; set; } = new List<LaunchLibraryAgency>();

        public string Configuration { get; set; }

        public string FamilyName { get; set; }

        public int ID { get; set; }

        public List<int> ImageSizes { get; set; } = new List<int>();

        public string ImageUrl { get; set; }

        public List<string> InfoUrls { get; set; } = new List<string>();

        public string Name { get; set; }

        public string WikiUrl { get; set; }

        public Rocket(dynamic item)
        {
            Configuration = Convert.ToString(item.configuration);
            FamilyName = Convert.ToString(item.familyname);
            ID = Convert.ToInt32(item.id);
            ImageUrl = Convert.ToString(item.imageURL);
            Name = Convert.ToString(item.name);
            WikiUrl = Convert.ToString(item.wikiURL);

            if (item.agencies != null)
            {
                for (var i = 0; i < item.agencies.Count; i++)
                {
                    Agencies.Add(new LaunchLibraryAgency(item.agencies[i]));
                }
            }

            if (item.imageSizes != null)
            {
                for (var i = 0; i < item.imageSizes.Count; i++)
                {
                    ImageSizes.Add(Convert.ToInt32(item.imageSizes[i]));
                }
            }

            if (item.infoURLs != null)
            {
                for (var i = 0; i < item.infoURLs.Count; i++)
                {
                    this.InfoUrls.Add(Convert.ToString(item.infoURLs[i]));
                }
            }
        }
    }
}
