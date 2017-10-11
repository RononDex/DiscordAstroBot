using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Objects.Config;

namespace DiscordAstroBot.TimerJobs
{
    /// <summary>
    /// Base class for TimerJobs. A TimerJob represents a job that has to be run regularly
    /// </summary>
    public abstract class TimerJobBase
    {
        /// <summary>
        /// Name of the timer job
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Returns the next time this Job has to be executed
        /// </summary>
        public abstract DateTime NextExecutionTime { get; }

        /// <summary>
        /// Gets set from the framework when the last execution was
        /// </summary>
        public DateTime? LastExecutionTime
        {
            get
            {
                var configEntry = Mappers.Config.TimerJobExecutions.Config.TimerJobEntries.FirstOrDefault(x => x.TimerJobName == Name);
                return configEntry?.LastExecution;
            }
            set
            {
                var configEntry = Mappers.Config.TimerJobExecutions.Config.TimerJobEntries.FirstOrDefault(x => x.TimerJobName == Name);
                if (configEntry == null)
                {
                    configEntry = new TimerJobEntry();
                    configEntry.TimerJobName = Name;
                    Mappers.Config.TimerJobExecutions.Config.TimerJobEntries.Add(configEntry);
                }

                if (value != null)
                    configEntry.LastExecution = value.Value;

                Mappers.Config.TimerJobExecutions.SaveConfig();
            }
        }

        /// <summary>
        /// Execute the scheduled timer job
        /// </summary>
        /// <param name="guild"></param>
        public abstract void Execute(IGuild guild);
    }
}
