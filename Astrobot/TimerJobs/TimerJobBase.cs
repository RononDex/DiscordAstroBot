using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.TimerJobs
{
    /// <summary>
    /// Base class for TimerJobs. A TimerJob represents a job that has to be run regularly
    /// </summary>
    public abstract class TimerJobBase
    {
        /// <summary>
        /// Returns the next time this Job has to be executed
        /// </summary>
        public abstract DateTime NextExecutionTime { get; }

        /// <summary>
        /// Gets set from the framework when the last execution was
        /// </summary>
        public DateTime? LastExecutionTime { get; set; } = null;

        /// <summary>
        /// Execute the scheduled timer job
        /// </summary>
        /// <param name="guild"></param>
        public abstract void Execute(IGuild guild);
    }
}
