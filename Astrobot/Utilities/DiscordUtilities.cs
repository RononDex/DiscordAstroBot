using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Utilities
{
    public static class DiscordUtilities
    {
        /// <summary>
        /// Tries to resolve a username on the given discord server
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Discord.User ResolveUser(Discord.Server server, string userName)
        {
            var user = server.Users.FirstOrDefault(x => x.Name.ToLower().Contains(userName));

            return user;
        }
    }
}
