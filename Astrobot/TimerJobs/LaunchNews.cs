using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordAstroBot.TimerJobs
{
    /// <summary>
    /// Timer Job to post daily news on astronomy
    /// </summary>
    public class LaunchNews : TimerJobBase
    {
        public override DateTime NextExecutionTime => LastExecutionTime != null
            ? LastExecutionTime.Value + new TimeSpan(24, 0, 0)
            : DateTime.Today.AddDays(1).AddHours(-2);

        public override string Name => "News";

        public async override void Execute(IGuild guild)
        {
            // Check if news enabled and if news channel is defined on the server
            var serverCfg = Mappers.Config.ServerConfig.Config.Servers.First(x => x.ServerID == guild.Id);

            if (serverCfg.Configs.First(x => x.Key == "BotLaunchNewsEnabled").Value.ToLower() == "true" &&
                !string.IsNullOrEmpty(serverCfg.Configs.First(x => x.Key == "BotNewsChannel").Value))
            {
                var channels = await guild.GetChannelsAsync(CacheMode.CacheOnly);
                var channel = channels.FirstOrDefault(x => x.Name == serverCfg.Configs.FirstOrDefault(y => y.Key == "BotNewsChannel").Value);

                if (channel != null)
                {
                    // Get the upcomming launches
                    var launches = Mappers.LaunchLibrary.LaunchLibraryAPIHelper.GetLaunches(DateTime.Today, DateTime.Today.AddDays(3));
                    
                    if (launches.Count > 0)
                    {
                        await (channel as ISocketMessageChannel).SendMessageAsync("Following launches are scheduled for the next 3 days:");

                        foreach (var launch in launches)
                        {
                            var message = "```python\r\n";
                            message += $"Mission-Name:      {launch.Name}\r\n";
                            message += $"Launch window:     {launch.WindowStart} - {launch.WindowEnd}\r\n";
                            message += $"Rocket:            {launch.Rocket.Name}\r\n";
                            message += $"Launch-Location:   {launch.Location.Name} - Pad: {launch.Location.LaunchPads.FirstOrDefault()}\r\n";
                            message += $"Wath live here:    {launch.VidURL}\r\n";
                            message += $"Description:\r\n{string.Join("\r\n - ", launch.Missions.Select(x => x.Description))}\r\n";
                            message += $"/*-----------------------------------------------------------------------------------------------------*/\r\n```";
                            await (channel as ISocketMessageChannel).SendMessageAsync(message);
                        }
                    }
                }
            }
        }
    }
}
