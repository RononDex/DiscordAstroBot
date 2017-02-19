using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects
{
    public class SpaceLaunch
    {
        public string FailReason { get; set; }

        public string Hashtag { get; set; }

        public string Holdreason { get; set; }

        public int ID { get; set; }

        public string InfoURL { get; set; }

        public List<string> InfoURLs { get; set; } = new List<string>();

        public bool InHold { get; set; }

        public LaunchLocation Location { get; set; }

        public List<SpaceMission> Missions { get; set; }

        public Rocket Rocket { get; set; }

        public LaunchStatus Status { get; set; }

        public string VidURL { get; set; }

        public List<string> VidURLs { get; set; } = new List<string>();

        public DateTime WindowEnd { get; set; }

        public DateTime WindowStart { get; set; }

        public string Name { get; set; }

        public float Probability { get; set; }

        public bool TBDDate { get; set; }

        public bool TBDTime { get; set; }

        public SpaceLaunch(dynamic item)
        {
            this.FailReason = item.failreason;
            this.Hashtag = item.hashtag;
            this.Holdreason = item.holdreason;
            this.ID = item.id;
            this.InfoURL = item.infoURL;
            this.InHold = Convert.ToBoolean(item.inhold);
            this.Name = item.name;
            this.Probability = Convert.ToSingle(item.probability);
            this.Status = (LaunchStatus)item.status;
            this.VidURL = item.vidURL;
            this.WindowEnd = DateTime.ParseExact(Convert.ToString(item.windowend), "MMMM dd, yyyy hh:mm:ss UTC", CultureInfo.InvariantCulture);
            this.WindowStart = DateTime.ParseExact(Convert.ToString(item.windowstart), "MMMM dd, yyyy hh:mm:ss UTC", CultureInfo.InvariantCulture); ;
            this.TBDDate = Convert.ToBoolean(item.tbddate);
            this.TBDTime = Convert.ToBoolean(item.tbdtime);

            this.Location = new LaunchLocation(item.location);

            if (item.vidURLs != null)
            {
                for (var i = 0; i < item.vidURLs.Count; i++)
                {
                    this.VidURLs.Add(Convert.ToString(item.vidURLs[i]));
                }
            }

            if (item.infoURLs != null)
            {
                for (var i = 0; i < item.infoURLs.Count; i++)
                {
                    this.InfoURLs.Add(Convert.ToString(item.infoURLs[i]));
                }
            }
        }
    }

    public enum LaunchStatus
    {
        GREEN = 1,
        RED = 2,
        SUCCESS = 3,
        FAILED = 4
    }
}
