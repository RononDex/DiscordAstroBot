using InstaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.SocialMedia
{
    /// <summary>
    /// SocialMediaProvider for instagram
    /// </summary>
    public class InstagramProvider : SocialMediaProviderBase
    {
        public InstagramProvider()
        {
            // Read the credentials file for instagramm and extract the user / password from it
            if (!File.Exists(ConfigurationManager.AppSettings["InstagramAPIKeyPath"]))
                throw new ArgumentException($"The file {ConfigurationManager.AppSettings["InstagramAPIKeyPath"]} does not exist or can not be accessed!");

            var paramsDict = new Dictionary<string, string>();

            var fileText = File.ReadAllLines(ConfigurationManager.AppSettings["InstagramAPIKeyPath"]);
            if (fileText.Length < 2)
                throw new ArgumentException($"The file holding the credentials for instagramm has less 2 lines (needs at least username and password)");

            paramsDict.Add("user", fileText[0]);
            paramsDict.Add("password", fileText[1]);

            Initialize(paramsDict);
        }

        /// <summary>
        /// Converts a string to a secure string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(string source)
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

        }
    }
}
