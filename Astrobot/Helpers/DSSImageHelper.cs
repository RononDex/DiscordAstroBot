using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

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
        public static byte[] GetImage(float ra, float dec, string size = "60", string mimetype = "download-gif", string catalogue = "DSS2")
        {
            var client = new WebClient();
            var data   = client.DownloadData($"{ServiceURL}?ra={HttpUtility.UrlEncode(ra.ToString())}&dec={HttpUtility.UrlEncode(dec.ToString())}&x={HttpUtility.UrlEncode(size)}&y={HttpUtility.UrlEncode(size)}&mime-type={HttpUtility.UrlEncode(mimetype)}&Sky-Survey={HttpUtility.UrlEncode(catalogue)}&equinox=J2000&statsmode=VO");

            using (var stream = new MemoryStream(data))
            {
                var image    = new Bitmap(stream);
                var newImage = Utilities.ImageUtility.MakeGrayscaleFromRGB_R(image);
                var stream2  = new MemoryStream();

                newImage.Save(stream2, ImageFormat.Jpeg);
                stream2.Position = 0;

                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = stream2.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}
