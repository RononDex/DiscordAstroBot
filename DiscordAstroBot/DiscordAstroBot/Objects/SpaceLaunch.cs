using System;
using System.Collections.Generic;
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

        public string[] InfoURLs { get; set; }

        public bool InHold { get; set; }

        public LaunchLocation Location { get; set; }

        public List<SpaceMission> Missions { get; set; }

        public Rocket Rocket { get; set; }

        public LaunchStatus Status { get; set; }

        public string VidURL { get; set; }

        public string[] VidURLs { get; set; }

        public DateTime WindowEnd { get; set; }

        public DateTime WindowStart { get; set; }

        public string Name { get; set; }

        public float Probability { get; set; }


        public SpaceLaunch(dynamic item)
        {
            this.FailReason = item.failreason;
            this.Hashtag = item.hashtag;
            this.Holdreason = item.holdreason;
            this.ID = item.id;
            this.InfoURL = item.infoURL;
            this.InfoURLs = item.infoURLs;
            this.InHold = Convert.ToBoolean(item.inhold);
            this.Name = item.name;
            this.Probability = Convert.ToSingle(item.probability);
            this.Status = (LaunchStatus)item.status;
            this.VidURL = item.vidURL;
            this.VidURLs = item.vidURLs;
            this.WindowEnd = Convert.ToDateTime(item.windowend);
            this.WindowStart = Convert.ToDateTime(item.windowend);
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
