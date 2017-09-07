using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.TimerJobs
{
    /// <summary>
    /// Timer Job to post daily news on astronomy
    /// </summary>
    public class News : TimerJobBase
    {
        public override DateTime NextExecutionTime => throw new NotImplementedException();

        public override void Execute(IGuild guild)
        {
            throw new NotImplementedException();
        }
    }
}
