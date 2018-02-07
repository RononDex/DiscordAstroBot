using DiscordAstroBot.Helpers;
using InstaSharp;
using InstaSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAstroBot.SocialMedia
{
    /// <summary>
    /// SocialMediaProvider for instagram
    /// </summary>
    public class InstagramProvider : SocialMediaProviderBase
    {
        private bool UploadCompleted { get; set; } = false;

        private string UploadedPostUrl { get; set; }

        public override string Name => "Instagram";

        public InstagramProvider()
        {
            // Read the credentials file for instagramm and extract the user / password from it
            if (!File.Exists(ConfigurationManager.AppSettings["InstagramAPIKeyPath"]))
                throw new ArgumentException($"The file {ConfigurationManager.AppSettings["InstagramAPIKeyPath"]} does not exist or can not be accessed!");

            var paramsDict = new Dictionary<string, string>();

            var fileText = File.ReadAllLines(ConfigurationManager.AppSettings["InstagramAPIKeyPath"]);
            if (fileText.Length < 2)
                throw new ArgumentException($"The file holding the credentials for instagramm has less than 2 lines (needs at least username and password)");

            paramsDict.Add("user", fileText[0]);
            paramsDict.Add("password", fileText[1]);

            Initialize(paramsDict);
        }

        /// <summary>
        /// Converts a string to a secure string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static SecureString ToSecureString(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            else
            {
                SecureString result = new SecureString();
                foreach (char c in source.ToCharArray())
                    result.AppendChar(c);
                return result;
            }
        }

        /// <summary>
        /// Publishes the post to instagramm
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public override string PublishPost(SocialMediaPost post)
        {
            var uploader = new InstagramUploader(Parameters["user"], ToSecureString(Parameters["password"]));

            //var image = new System.Net.WebClient().DownloadData(post.ImageUrl);

            var userSettings = SocialMediaHelper.GetUserSocialMediaSettings(post.ServerID, post.Author);
            var instagramAuthorHandle = userSettings.InstagramUserName;
            var serverSettings = Mappers.Config.ServerConfig.GetServerSetings(post.ServerID);
            var serverTags = string.Join(" ", serverSettings.Configs.FirstOrDefault(x => x.Key == "SocialMediaPublishingInstagramHashtags").Value?.Split(';'));
            var userTags = userSettings.InstagramHashtags;

            var content = $"{post.Content}\r\n\r\nImage credit: @{instagramAuthorHandle}\r\n\r\n"
                 + $"{serverTags} {userTags}";

            uploader.OnCompleteEvent += Uploader_OnCompleteEvent;
            uploader.UploadImage(post.ImageUrl, content, false, true);

            var waitStep = 100;
            var maxWait = 120000;
            var curWait = 0;

            while (!this.UploadCompleted)
            {
                if (curWait > maxWait)
                    throw new TimeoutException("The upload to instagram timed out!");

                Thread.Sleep(waitStep);

                curWait += waitStep;
            }

            return this.UploadedPostUrl;
        }

        private void Uploader_OnCompleteEvent(object sender, EventArgs e)
        {
            Console.WriteLine("Image posted to Instagram, here are all the urls");
            foreach (var image in ((UploadResponse)e).Images)
            {
                this.UploadedPostUrl = image.Url;
                this.UploadCompleted = true;
                break; // Only store first image (bot can only post 1 image at a time either way)
            }
        }
    }
}
