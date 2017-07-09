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
    }
}
