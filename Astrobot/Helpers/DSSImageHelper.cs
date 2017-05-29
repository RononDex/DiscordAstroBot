using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Extensions.MonoHttp;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Helper to get images from the deep space imageing survey DSS
    /// </summary>
    public static class DSSImageHelper
    {
        const string ServiceURL = "http://archive.eso.org/dss/dss/image";

        /// <summary>
        /// Gets an image from the DSS
        /// </summary>
        /// <param name="ra">RA coordinates of target</param>
        /// <param name="size">Size of image in arcminutes, default: 85</param>
        /// <param name="mimetype">mime type of image to downlaod, default: download-gif</param>
        /// <param name="catalogue">the catalogue to query, default: DSS2</param>
        /// <returns></returns>
        public static byte[] GetImage(string ra, string dec, string size = "60", string mimetype = "download-gif", string catalogue = "DSS2")
        {
            var client = new WebClient();
            var data = client.DownloadData($"{ServiceURL}?ra={HttpUtility.UrlEncode(ra)}&dec={HttpUtility.UrlEncode(dec)}&x={HttpUtility.UrlEncode(size)}&y={HttpUtility.UrlEncode(size)}&mime-type={HttpUtility.UrlEncode(mimetype)}&Sky-Survey={HttpUtility.UrlEncode(catalogue)}&equinox=J2000&statsmode=VO");

            return data;
        }
    }
}
