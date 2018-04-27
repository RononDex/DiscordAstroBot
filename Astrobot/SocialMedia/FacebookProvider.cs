using DiscordAstroBot.Helpers;
using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.SocialMedia
{
    public class FacebookProvider : SocialMediaProviderBase
    {
        /// <summary>
        /// API Url used to create a new post on a page
        /// See https://developers.facebook.com/docs/graph-api/reference/page/photos
        /// </summary>
        private const string CREATE_PAGE_POST_URL = "{PageID}/photos";

        public override string Name => "Facebook";

        public FacebookProvider()
        {
            // Read the credentials file for instagramm and extract the user / password from it
            if (!File.Exists(ConfigurationManager.AppSettings["FacebookAccessTokenPath"]))
                throw new ArgumentException($"The file {ConfigurationManager.AppSettings["FacebookAccessTokenPath"]} does not exist or can not be accessed!");

            var paramsDict = new Dictionary<string, string>();

            var fileText = File.ReadAllLines(ConfigurationManager.AppSettings["FacebookAccessTokenPath"]);
            if (fileText.Length < 2)
                throw new ArgumentException($"The file holding the credentials for facebook has less than 2 lines (needs at least accesstoken and pageid)");

            paramsDict.Add("PageID", fileText[1]);
            paramsDict.Add("AppToken", fileText[0]);

            Initialize(paramsDict);
        }

        public override async Task<string> PublishPost(SocialMediaPost post)
        {
            var client = new FacebookClient(this.Parameters["AcessToken"]);

            var userSettings = SocialMediaHelper.GetUserSocialMediaSettings(post.ServerID, post.Author);
            var facebookUserHandle = userSettings.FacebookUserName;
            var content = "";
            if (!string.IsNullOrEmpty(facebookUserHandle))
                content = $"{post.Content}, Image credit: @{facebookUserHandle}";
            else
                content = $"{post.Content}\r\n\r\n";


            var urlSplitted = post.ImageUrl.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var fileName = urlSplitted[urlSplitted.Length - 2];

            var wc = new WebClient();
            var memstream = new MemoryStream(wc.DownloadData(post.ImageUrl));
            var copy = new MemoryStream();
            memstream.CopyTo(copy);

            dynamic parameters = new ExpandoObject();
            parameters.message = post.Content;
            parameters.url = post.ImageUrl;
            parameters.source = new FacebookMediaObject
            {
                ContentType = $"image/jpeg",
                FileName = fileName
            }.SetValue(copy.ToArray());

            var res = client.Post(CREATE_PAGE_POST_URL.Replace("{PageID}", this.Parameters["PageID"]), parameters);
            dynamic result = JsonConvert.DeserializeObject(res);

            return $"https://facebook.com/{result.post_id}";
        } 
    }
}
