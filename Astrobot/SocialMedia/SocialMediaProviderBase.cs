using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.SocialMedia
{
    /// <summary>
    /// Abstract class representing a provider for a social media plattform
    /// </summary>
    public abstract class SocialMediaProviderBase
    {
        protected IReadOnlyDictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Initialize the provider with some parameters
        /// </summary>
        /// <param name="Parameters"></param>
        public void Initialize(Dictionary<string, string> parameters)
        {
            this.Parameters = new ReadOnlyDictionary<string, string>(parameters);
        }

        /// <summary>
        /// Publish a post on to social media
        /// Should return the url of the post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public abstract string PublishPost(SocialMediaPost post);
    }

    /// <summary>
    /// Represents a post for social media
    /// </summary>
    public class SocialMediaPost
    {
        public string Content { get; set; }

        public byte[] Image { get; set; }
    }
}
