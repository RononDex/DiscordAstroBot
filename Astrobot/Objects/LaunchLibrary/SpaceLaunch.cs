using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects.LaunchLibrary
{
    /// <summary>
    /// Represents a space launch event
    /// </summary>
    public class SpaceLaunch
    {
        /// <summary>
        /// The reason for its failure
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        /// Hashtag used on social media
        /// </summary>
        public string Hashtag { get; set; }

        /// <summary>
        /// Reason for countdown hold
        /// </summary>
        public string Holdreason { get; set; }

        /// <summary>
        /// An unique ID for the space launch event
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// An URL where one can get further information
        /// </summary>
        public string InfoURL { get; set; }

        /// <summary>
        /// Further informations on the event
        /// </summary>
        public List<string> InfoURLs { get; set; } = new List<string>();

        /// <summary>
        /// Is the launch countdown on hold?
        /// </summary>
        public bool InHold { get; set; }

        /// <summary>
        /// The location of the launch
        /// </summary>
        public LaunchLocation Location { get; set; }

        /// <summary>
        /// The missions of this launch
        /// </summary>
        public List<SpaceMission> Missions { get; set; } = new List<SpaceMission>();

        /// <summary>
        /// The rocket used for the launch
        /// </summary>
        public Rocket Rocket { get; set; }

        /// <summary>
        /// Current LaunchStatus
        /// </summary>
        public LaunchStatus Status { get; set; }

        /// <summary>
        /// URL to Livestream
        /// </summary>
        public string VidURL { get; set; }

        /// <summary>
        /// Alternative livestreams
        /// </summary>
        public List<string> VidURLs { get; set; } = new List<string>();

        /// <summary>
        /// The end of the launch Window (UTC)
        /// </summary>
        public DateTime? WindowEnd { get; set; }

        /// <summary>
        /// The start of the launch window (UTC)
        /// </summary>
        public DateTime? WindowStart { get; set; }

        /// <summary>
        /// Planned date
        /// </summary>
        public DateTime Net { get; set; }

        /// <summary>
        /// The name of the launch
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Success probability, -1 if unknown
        /// </summary>
        public float Probability { get; set; }

        /// <summary>
        /// Date to be announced
        /// </summary>
        public bool TBDDate { get; set; }

        /// <summary>
        /// Time to be announced
        /// </summary>
        public bool TBDTime { get; set; }

        public SpaceLaunch(dynamic item)
        {
            // Initialize the object from the json object
            this.FailReason = item.failreason;
            this.Hashtag = item.hashtag;
            this.Holdreason = item.holdreason;
            this.ID = item.id;
            this.InfoURL = item.infoURL;
            this.InHold = Convert.ToBoolean(item.inhold);
            this.Name = item.name;
            this.Probability = Convert.ToSingle(item.probability);
            if (item.status != null)
                this.Status = (LaunchStatus)item.status;
            this.VidURL = item.vidURL;
            if (item.isoend != null)
            {
                try
                {
                    this.WindowEnd = DateTime.ParseExact(Convert.ToString(item.isoend), "yyyyMMdd\\THHmmss\\Z", CultureInfo.InvariantCulture);
                    this.WindowStart = DateTime.ParseExact(Convert.ToString(item.isostart), "yyyyMMdd\\THHmmss\\Z", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Log<DiscordAstroBot>.ErrorFormat($"Got invalid date format to parse: {item.item.isoend} or {item.isostart}: {ex.Message}");
                }
            }

            this.Net = DateTime.ParseExact(Convert.ToString(item.net), "MMMM d, yyyy HH:mm:ss UTC", CultureInfo.InvariantCulture);
            this.TBDDate = Convert.ToBoolean(item.tbddate);
            this.TBDTime = Convert.ToBoolean(item.tbdtime);

            if (item.location != null)
                this.Location = new LaunchLocation(item.location);

            if (item.rocket != null)
                this.Rocket = new Rocket(item.rocket);

            if (item.missions != null)
            {
                for (var i = 0; i < item.missions.Count; i++)
                {
                    Missions.Add(new SpaceMission(item.missions[i]));
                }
            }

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

    /// <summary>
    /// An enumeration representing the launch status
    /// </summary>
    public enum LaunchStatus
    {
        GREEN   = 1,
        RED     = 2,
        SUCCESS = 3,
        FAILED  = 4
    }
}
