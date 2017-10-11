using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects.Config
{
    /// <summary>
    /// Stores the last execution times of the individual Timer Jobs
    /// </summary>
    [Serializable, XmlRoot("TimerJobs"), XmlType("TimerJobs")]
    public class TimerJobExecutions
    {
        [XmlElement("TimerJob")]
        public List<TimerJobEntry> TimerJobEntries { get; set; }
    }

    public class TimerJobEntry
    {
        /// <summary>
        /// Name of the timer job
        /// </summary>
        [XmlAttribute("Name")]
        public string TimerJobName { get; set; }

        /// <summary>
        /// Last execution time of this timer job
        /// </summary>
        [XmlAttribute("LastExecuted")]
        public DateTime LastExecution { get; set; }
    }
}
