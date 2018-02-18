using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Runtime.Caching;

namespace DiscordAstroBot.TimerJobs
{
    /// <summary>
    /// Timer job used to inform people from intermediate launches
    /// </summary>
    public class IntermediateLaunchNotify : TimerJobBase
    {
        /// <summary>
        /// The name of this timer job
        /// </summary>
        public override string Name => "IntermediateLaunchNotify";

        /// <summary>
        /// Determines when this task has to be executed the next time
        /// </summary>
        public override DateTime NextExecutionTime => LastExecutionTime == null ? DateTime.Today : LastExecutionTime.Value.AddMinutes(5);

        /// <summary>
        /// Cache so the bot knows which launches haven been notified already
        /// </summary>
        public static List<string> NotifiedLaunches { get; set; } = new List<string>();

        /// <summary>
        /// Executes the timer job
        /// </summary>
        /// <param name="guild"></param>
        public async override Task Execute(IGuild guild)
        {
            try
            {
                var serverCfg = Mappers.Config.ServerConfig.Config.Servers.First(x => x.ServerID == guild.Id);

                // First check if intermediate launches are even enabled / configured on this server
                var channelName = serverCfg?.Configs.FirstOrDefault(x => x.Key == "BotNewsChannel").Value;

                //  If invalid config, channelName will be empty
                if (string.IsNullOrEmpty(channelName))
                {
                    //Log<DiscordAstroBot>.Warn($"Invalid BotNewsChannel Config found on server {guild.Name}");
                    return;
                }

                // Load the channel on the discord server
                var channels = await guild.GetChannelsAsync(CacheMode.CacheOnly);
                var channel = channels.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower() == channelName.ToLower());

                // Find the role to notify
                var roleName = serverCfg?.Configs.FirstOrDefault(x => x.Key == "IntermediateLaunchTagRole").Value;
                var role = guild.Roles.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower() == roleName.ToLower());

                if (!string.IsNullOrEmpty(channelName) && channel != null
                    && !string.IsNullOrEmpty(roleName) && role != null)
                {
                    // Search for intermediate launches
                    var upcomingLaunches = Mappers.LaunchLibrary.LaunchLibraryAPIHelper.GetNextLaunches();

                    var intermediateLaunches = upcomingLaunches.Where(x => x.WindowStart < DateTime.Now.AddHours(1) && x.WindowStart > DateTime.Now);

                    if (intermediateLaunches.Any())
                    {
                        foreach (var item in intermediateLaunches.Where(x => !NotifiedLaunches.Any(y => y == x.Name) ))
                        {
                            await (channel as ITextChannel).SendMessageAsync($"{role.Mention} upcoming launch within 1hour:\r\n**Name:** {item.Name}\r\nLivestream: {(!string.IsNullOrEmpty(item.VidURL) ? item.VidURL : item.VidURLs.FirstOrDefault())}");
                            NotifiedLaunches.Add(item.Name);
                        }
                    }
                    else
                    {
                        NotifiedLaunches.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Log<DiscordAstroBot>.Error($"Error while executing timer job {Name}: {ex.Message}");
            }
        }
    }
}
