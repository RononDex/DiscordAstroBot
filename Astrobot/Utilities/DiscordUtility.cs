﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Utilities
{
    public static class DiscordUtility
    {
        /// <summary>
        /// Tries to resolve a username on the given discord server
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static SocketUser ResolveUser(SocketGuild server, string userName)
        {
            var user = server.Users.FirstOrDefault(x => x.Username.ToLower().Contains(userName));

            return user;
        }

        /// <summary>
        /// Tries to resolve a channel on the given discord server
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<ITextChannel> ResolveChannel(IGuild server, string channelName)
        {
            var channels = await server.GetChannelsAsync(Discord.CacheMode.AllowDownload);
            var channel = channels.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(channelName.ToLower()));

            return channel as ITextChannel;
        }

        /// <summary>
        /// Logs a message into discord channel
        /// </summary>
        /// <param name="message"></param>
        /// <param name="server"></param>
        public static async void LogToDiscord(string message, IGuild server)
        {
            // Check if discord logging is enabled for the server
            if (string.IsNullOrEmpty(Mappers.Config.ServerConfig.Config.Servers.First(x => x.ServerID == server.Id).Configs.FirstOrDefault(x => x.Key == "BotLogChannel").Value))
                return;

            // Find the logging channel on the server
            var channels = await server.GetChannelsAsync(Discord.CacheMode.CacheOnly);
            var logChannel = channels.FirstOrDefault(x => x.Name == Mappers.Config.ServerConfig.Config.Servers.First(y => y.ServerID == server.Id).Configs.FirstOrDefault(y => y.Key == "BotLogChannel").Value) as Discord.ITextChannel;

            if (logChannel == null)
                return;

            await logChannel.SendMessageAsync(message);
        }
    }
}
