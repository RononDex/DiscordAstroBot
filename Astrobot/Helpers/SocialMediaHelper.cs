using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Used to help with social media commands
    /// </summary>
    public static class SocialMediaHelper
    {
        /// <summary>
        /// Enables social media publishing for a certain user on the given server
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        public static void EnableSocialMediaPublishingForUser(ulong serverID, ulong userID)
        {
            var server = EnsureServerEntryExists(serverID);

            var user = EnsureUserEntryExists(server, userID);

            user.SocialMediaPublishingEnabled = true;
            Mappers.Config.SocialMediaConfig.SaveConfig();
        }

        /// <summary>
        /// Disables the social media publishing feature for the given user
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        public static void DisableSocialMediaPublishingForUser(ulong serverID, ulong userID)
        {
            var server = EnsureServerEntryExists(serverID);

            var user = EnsureUserEntryExists(server, userID);

            user.SocialMediaPublishingEnabled = false;
            Mappers.Config.SocialMediaConfig.SaveConfig();
        }

        /// <summary>
        /// Sets the InstagramUser handle for social media publishing for the given user
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        public static void SetInstagrammUser(ulong serverID, ulong userID, string instagramUser)
        {
            var server = EnsureServerEntryExists(serverID);

            var user = EnsureUserEntryExists(server, userID);

            user.InstagramUserName = instagramUser.Trim().Replace("@", "");
            Mappers.Config.SocialMediaConfig.SaveConfig();
        }

        /// <summary>
        /// Sets the InstagramUser handle for social media publishing for the given user
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        public static void SetInstagrammUserHastTags(ulong serverID, ulong userID, string tags)
        {
            var server = EnsureServerEntryExists(serverID);

            var user = EnsureUserEntryExists(server, userID);

            user.InstagramHashtags = tags;
            Mappers.Config.SocialMediaConfig.SaveConfig();
        }


        /// <summary>
        /// Sets the InstagramUser handle for social media publishing for the given user
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        public static void SetFacebookUser(ulong serverID, ulong userID, string facebookUser)
        {
            var server = EnsureServerEntryExists(serverID);

            var user = EnsureUserEntryExists(server, userID);

            user.FacebookUserName = facebookUser.Trim();
            Mappers.Config.SocialMediaConfig.SaveConfig();
        }

        /// <summary>
        /// Gets the current social media config for this user
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static Objects.Config.SocialMediaServerUserConfig GetUserSocialMediaSettings(ulong serverID, ulong userID)
        {
            var server = EnsureServerEntryExists(serverID);

            var user = EnsureUserEntryExists(server, userID);
            return user;
        }

        /// <summary>
        /// Ensures that the given user entry exists on the server, creates it if it doesnt exist yet
        /// </summary>
        /// <param name="server"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private static Objects.Config.SocialMediaServerUserConfig EnsureUserEntryExists(Objects.Config.SocialMediaServerConfig server, ulong userID)
        {
            var userEntry = server.UserEntries.FirstOrDefault(x => x.User == userID);

            // If user entry is null, create it
            if (userEntry == null)
            {
                userEntry = new Objects.Config.SocialMediaServerUserConfig();
                userEntry.User = userID;
                server.UserEntries.Add(userEntry);
                Mappers.Config.SocialMediaConfig.SaveConfig();
            }

            return userEntry;
        }

        /// <summary>
        /// Ensures the given server entry exists in the config (creates one if it does not exist yet).
        /// Returns the proper server entry
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        private static Objects.Config.SocialMediaServerConfig EnsureServerEntryExists(ulong serverID)
        {
            var server = Mappers.Config.SocialMediaConfig.Config.ServerEntries.FirstOrDefault(x => x.ServerID == serverID);

            // If server entry does not exist yet, create it!
            if (server == null)
            {
                server = new Objects.Config.SocialMediaServerConfig();
                server.ServerID = serverID;
                Mappers.Config.SocialMediaConfig.Config.ServerEntries.Add(server);
                Mappers.Config.SocialMediaConfig.SaveConfig();
            }

            return server;
        }
    }
}
