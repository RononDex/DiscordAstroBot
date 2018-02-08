using DiscordAstroBot.Helpers;
using DiscordAstroBot.Utilities;
using InstaSharper;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
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
        public override async Task<string> PublishPost(SocialMediaPost post)
        {
            var api = InstaApiBuilder.CreateBuilder()
                .SetUser(new InstaSharper.Classes.UserSessionData() { UserName = Parameters["user"], Password = Parameters["password"] })
                .Build();
            

            var loggedIn = await api.LoginAsync();

            var image = new System.Net.WebClient().DownloadData(post.ImageUrl);
            var tempFile = Path.GetTempFileName();
            var jpgFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, image);
            ImageUtility.ConvertImageToJpg(tempFile, jpgFile);

            var userSettings = SocialMediaHelper.GetUserSocialMediaSettings(post.ServerID, post.Author);
            var instagramAuthorHandle = userSettings.InstagramUserName;
            var serverSettings = Mappers.Config.ServerConfig.GetServerSetings(post.ServerID);
            var serverTags = string.Join(" ", serverSettings.Configs.FirstOrDefault(x => x.Key == "SocialMediaPublishingInstagramHashtags").Value?.Split(';'));
            var userTags = userSettings.InstagramHashtags;

            var content = $"{post.Content}\r\n\r\nImage credit: @{instagramAuthorHandle}\r\n\r\n"
                 + $"{serverTags} {userTags}";

            var res = await api.UploadPhotoAsync(new InstaSharper.Classes.Models.InstaImage() { URI = jpgFile }, content);

            // Clean up the temp file
            File.Delete(tempFile);
            File.Delete(jpgFile);

            return $"https://www.instagram.com/p/{res.Value.Code}/";
        }
    }
}
