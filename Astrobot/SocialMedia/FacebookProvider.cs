using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string CREATE_PAGE_POST_URL = "https://graph.intern.facebook.com/v2.11/me/photos?access_token={ACCESSTOKEN}&published=true&url={IMAGEURL}&caption={CONTENT}";

        public override string Name => "Facebook";

        public override async Task<string> PublishPost(SocialMediaPost post)
        {
            return string.Empty;
        } 
    }
}
