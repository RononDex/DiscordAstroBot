using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Astrometry helper class
    /// </summary>
    public static class AstrometryHelper
    {
        public static string LoginIntoAstrometry(string token)
        {
            Log<DiscordAstroBot>.InfoFormat("Login into Astrometry...");

            // Setup json payload
            var json = new { apikey = token};

            var webRequest = (HttpWebRequest)WebRequest.Create("http://nova.astrometry.net/api/login");
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            var requestStream = webRequest.GetRequestStream();

            // Send the json payload
            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write( string.Format("request-json={0}", WebUtility.UrlEncode(JsonConvert.SerializeObject(json))));
                streamWriter.Flush();
            }

            var response = (HttpWebResponse)webRequest.GetResponse();
            
            // Get answer from server
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            dynamic jsonResult = JsonConvert.DeserializeObject(text);
            if (jsonResult.status != "success")
                throw new Exception("Login was refused by Astrometry API");

            Log<DiscordAstroBot>.InfoFormat("Login into Astrometry successfull. SessionKey: {0}", jsonResult.session);

            return jsonResult.session;
        }

        public static string HttpUploadFile(string url, Stream file, string fileName, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, fileName, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            Stream fileStream = file;
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                return reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return null;
        }

        public static string UploadFile(string fileUrl, string fileName, string sessionID)
        {
            Log<DiscordAstroBot>.InfoFormat("Submitting a file to astrometry (SessionID: {0}, File: {1})", sessionID, fileUrl);

            // Setup json payload
            var json = new { session = sessionID, allow_commercial_use = "n", allow_modifications = "n", publicly_visible = "y" };

            var wc = new WebClient();
            var memstream = new MemoryStream(wc.DownloadData(fileUrl));

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("request-json", JsonConvert.SerializeObject(json));

            // Get answer from server
            string text = HttpUploadFile("http://nova.astrometry.net/api/upload", memstream, fileName, "file", "application/octet-stream", nvc);

            dynamic jsonResult = JsonConvert.DeserializeObject(text);
            if (jsonResult.status != "success")
                throw new Exception("Submitting your file to Astrometry failed.");

            Log<DiscordAstroBot>.InfoFormat("Submission was successfull (SessionID: {0}, File: {1}, SubmissionID: {2})", jsonResult.session, fileUrl, jsonResult.subid);

            return jsonResult.subid;
        }
    }
}
