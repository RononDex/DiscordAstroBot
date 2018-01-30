using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// Commands for social media interaction / publishing
    /// </summary>
    public class SocialMediaCommands : Command
    {
        /// <summary>@
        /// The name of the command
        /// </summary>
        public override string CommandName => "SocialMedia";

        /// <summary>
        /// Synonyms that trigger the command
        /// </summary>
        public override CommandSynonym[] CommandSynonyms => new[] {
            new CommandSynonym() { Synonym = "(enable|actrivate) social([-\\s])?media([-\\s])?publishing(?'EnableSocialMediaPublishingUser')" },
            new CommandSynonym() { Synonym = "(disable|deactivate) social([-\\s])?media([-\\s])?publishing(?'DisableSocialMediaPublishingUser')" },
            new CommandSynonym() { Synonym = "set (my )?instagram user(\\s)?(name)? to (?'SetInstagramUserName'.*)" },
            new CommandSynonym() { Synonym = "set (my )?facebook user(\\s)?(name)? to (?'SetFacebookUserName'.*)" },
            new CommandSynonym() { Synonym = "show (me )?my social([-\\s])?media([-\\s])?(publishing )?settings(?'ShowSocialMediaSettings')" }
        };

        /// <summary>
        /// The description of this command, that is displayed when the help command is used
        /// </summary>
        public override string Description => $"Social media specific commands. Used for auto publishing of gallery posts\r\nUsage:\r\n```    @Astro Bot enable/disable social media publishing\r\n    @Astro Bot set instagram/facebook username to <Username>\r\n```";

        /// <summary>
        /// Gets called when this command gets triggered
        /// </summary>
        /// <param name="matchedMessage"></param>
        /// <param name="recievedMessage"></param>
        /// <returns></returns>
        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            // Enable social media publishing
            if (matchedMessage.Groups["EnableSocialMediaPublishingUser"].Success)
            {
                if (!IsSocialMediaPublishingActivated(recievedMessage)) return true;

                Helpers.SocialMediaHelper.EnableSocialMediaPublishingForUser((recievedMessage.Channel as SocketGuildChannel).Id, recievedMessage.Author.Id);
                await recievedMessage.Channel.SendMessageAsync("Social media publishing is now **activated** for your account!");
            }

            // Disable social media publishing
            if (matchedMessage.Groups["DisableSocialMediaPublishingUser"].Success)
            {
                if (!IsSocialMediaPublishingActivated(recievedMessage)) return true;

                Helpers.SocialMediaHelper.DisableSocialMediaPublishingForUser((recievedMessage.Channel as SocketGuildChannel).Id, recievedMessage.Author.Id);
                await recievedMessage.Channel.SendMessageAsync("Social media publishing is now **deactivated** for your account!");
            }

            // Set instagram user name
            if (matchedMessage.Groups["SetInstagramUserName"].Success)
            {
                if (!IsSocialMediaPublishingActivated(recievedMessage)) return true;

                Helpers.SocialMediaHelper.SetInstagrammUser((recievedMessage.Channel as SocketGuildChannel).Id, recievedMessage.Author.Id, matchedMessage.Groups["SetInstagramUserName"].Value);
                await recievedMessage.Channel.SendMessageAsync($"Instagram user name is now set to **\"{matchedMessage.Groups["SetInstagramUserName"].Value}\"**");
            }

            // Set instagram user name
            if (matchedMessage.Groups["SetFacebookUserName"].Success)
            {
                if (!IsSocialMediaPublishingActivated(recievedMessage)) return true;

                Helpers.SocialMediaHelper.SetFacebookUser((recievedMessage.Channel as SocketGuildChannel).Id, recievedMessage.Author.Id, matchedMessage.Groups["SetFacebookUserName"].Value);
                await recievedMessage.Channel.SendMessageAsync($"Facebook user name is now set to **\"{matchedMessage.Groups["SetFacebookUserName"].Value}\"**");
            }

            // Displays the current social media settings for the user
            if (matchedMessage.Groups["ShowSocialMediaSettings"].Success)
            {
                if (!IsSocialMediaPublishingActivated(recievedMessage)) return true;

                var settings = Helpers.SocialMediaHelper.GetUserSocialMediaSettings((recievedMessage.Channel as SocketGuildChannel).Id, recievedMessage.Author.Id);
                await recievedMessage.Channel.SendMessageAsync($"Your current social media settings are:\r\n```\r\nEnabled: {settings.SocialMediaPublishingEnabled}\r\nInstagram user: {settings.InstagramUserName}\r\nFacebook user: {settings.FacebookUserName}\r\n```");
            }

            return true;
        }

        /// <summary>
        /// Checks wether social media publishing is activated on the current server and writes an error message if not active
        /// </summary>
        /// <param name="recievedMessage"></param>
        /// <returns></returns>
        private bool IsSocialMediaPublishingActivated(SocketMessage recievedMessage)
        {
            var msg = "Social media publishing is not active on this server!";
            var serverConfig = Mappers.Config.ServerConfig.Config.Servers.FirstOrDefault(x => x.ServerID == (recievedMessage.Channel as SocketGuildChannel).Id);

            if (serverConfig == null)
            {
                recievedMessage.Channel.SendMessageAsync(msg);
                return false;
            }

            if (!Convert.ToBoolean(serverConfig.Configs.First(x => x.Key == "SocialMediaPublishingEnabled").Value))
            {
                recievedMessage.Channel.SendMessageAsync(msg);
                return false;
            }

            return true;
        }
    }
}
