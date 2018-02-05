using DiscordAstroBot.Objects.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace DiscordAstroBot.Mappers.Config
{
    public static class SocialMediaModQueue
    {
        /// <summary>
        /// Path to the config file
        /// </summary>
        public const string XmlFile = "config/SocialMediaModQueue.xml";

        /// <summary>
        /// The configuration of the commands per server
        /// </summary>
        public static Objects.Config.SocialMediaModQueueConfig Config { get; set; }

        /// <summary>
        /// Loads the config
        /// </summary>
        public static void LoadConfig()
        {
            Config = XmlSerialization.XmlStateController.LoadObject<Objects.Config.SocialMediaModQueueConfig>(XmlFile);
        }

        /// <summary>
        /// Saves the config to the xml file
        /// </summary>
        public static void SaveConfig()
        {
            XmlSerialization.XmlStateController.SaveObject(Config, XmlFile);
        }

        /// <summary>
        /// Gets the moderation queue for the given server
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public static Objects.Config.SocialMediaModQueueServerEntry GetModQueueForServer(ulong serverID)
        {
            var serverEntry = Config.Servers.FirstOrDefault(x => x.ID == serverID);

            // If the server entry does not exist yet, create one
            if (serverEntry == null)
            {
                serverEntry = new Objects.Config.SocialMediaModQueueServerEntry();
                serverEntry.ID = serverID;
                serverEntry.QueueEntries = new List<Objects.Config.SocialMediaModQueueEntry>();
                Config.Servers.Add(serverEntry);
                SaveConfig();
            }

            return serverEntry;
        }

        /// <summary>
        /// Gets the mod queue entry with the given id.
        /// Return NULL if entry does not exist
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Objects.Config.SocialMediaModQueueEntry GetEntryById(ulong serverId, ulong id)
        {
            var serverEntry = GetModQueueForServer(serverId);
            return serverEntry.QueueEntries.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Gets a list of pending entries from the moderation queue
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public static List<Objects.Config.SocialMediaModQueueEntry> GetPendingEntries(ulong serverID)
        {
            var serverEntry = GetModQueueForServer(serverID);
            return serverEntry.QueueEntries.Where(x => x.Status == Objects.Config.SocialMediaModQueueStatus.AWAITINGREVIEW).ToList();
        }

        /// <summary>
        /// Creates a new entry in the mod queue
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="postID"></param>
        /// <param name="imageUrl"></param>
        /// <param name="contents"></param>
        public static async Task<SocialMediaModQueueEntry> CreateNewModQueueEntry(ulong serverID, ulong postID, SocketUser author, string imageUrl, string contents, ITextChannel modChannel)
        {
            var serverEntry = GetModQueueForServer(serverID);
            var newEntry = new Objects.Config.SocialMediaModQueueEntry()
            {
                Author = author.Id,
                Content = contents,
                ID = postID,
                ImageUrl = imageUrl,
                Status = Objects.Config.SocialMediaModQueueStatus.AWAITINGREVIEW
            };

            serverEntry.QueueEntries.Add(newEntry);
            SaveConfig();

            // Notify mods on the moderation channel that a new post is awaiting moderation
            if (modChannel != null)
                await modChannel.SendMessageAsync($"New social media post awaiting moderation:\r\n{newEntry}");

            // Send private message to user
            var dmChannel = await author.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync($"Your new post with ID **{postID}** is now awaiting moderation to be published to social media.\r\nIf you have questions during the process please provide this ID as reference for the mods");

            return newEntry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="postId"></param>
        public static async void ApprovePost(ulong serverId, ulong entryId, SocketMessage recievedMessage)
        {
            // First, check if the user has the permissions for this command
            var guildUser = recievedMessage.Author as SocketGuildUser;
            var serverSettings = Mappers.Config.ServerConfig.GetServerSetings(serverId);

            if (!guildUser.Roles.Any(x => x.Name.ToLower() == serverSettings.Configs.FirstOrDefault(y => y.Key == "SocialMediaPublishingModGroup").Value.ToLower()))
            {
                await recievedMessage.Channel.SendMessageAsync("WARNING! Security breach detected!\r\nYou don't have access to this command!");
                return;
            }

            var queueEntry = Mappers.Config.SocialMediaModQueue.GetEntryById(serverId, entryId);
            if (queueEntry == null)
            {
                await recievedMessage.Channel.SendMessageAsync($"No queue entry found with ID **{entryId}**");
                return;
            }

            if (queueEntry.Status != Objects.Config.SocialMediaModQueueStatus.AWAITINGREVIEW)
            {
                await recievedMessage.Channel.SendMessageAsync($"The queue entry with ID **{entryId}** has already been **{queueEntry.Status}**");
                return;
            }

            // Set the state of the entry to approved
            queueEntry.Status = SocialMediaModQueueStatus.APPROVED;
            SaveConfig();
            await recievedMessage.Channel.SendMessageAsync($"The entry with id {entryId} has been approved for social media");

            // Notify the author of the post
            var pmChannel = await recievedMessage.Author.GetOrCreateDMChannelAsync();
            await pmChannel.SendMessageAsync($"**Congratulations!**\r\nYour post {entryId} has been approved by the mods to appear on the social media account of server **{( recievedMessage.Channel as IGuildChannel).Guild.Name}**");
        }
    }
}
