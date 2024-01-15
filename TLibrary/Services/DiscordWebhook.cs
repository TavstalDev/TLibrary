using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Compatibility;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Web.UI.WebControls;
using Tavstal.TLibrary.Compatibility.Models.Discord;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Services
{
    /// <summary>
    /// Discord webhook class used to send messages and images to discord servers
    /// </summary>
    public class DiscordWebhook
    {
        /// <summary>
        /// Discord Webhook Url
        /// </summary>
        private readonly string _webhookUrl;
        private readonly string _webhookName;
        private readonly string _webhookAvatarUrl;
        private static readonly Encoding _encoding = Encoding.UTF8;

        public DiscordWebhook(string url)
        {
            _webhookUrl = url;
        }

        public DiscordWebhook(string name, string url)
        {
            _webhookName = name;
            _webhookUrl = url;
        }

        public DiscordWebhook(string name, string avatar, string url)
        {
            _webhookName = name;
            _webhookAvatarUrl = avatar;
            _webhookUrl = url;
        }

        /// <summary>
        /// Function used to post mixed messages to discord
        /// </summary>
        /// <param name="message"></param>
        /// <param name="embed"></param>
        /// <param name="fileName"></param>
        /// <param name="fileFormat"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string Post(string message, Embed embed, string fileName, string fileFormat, string filePath)
        {
            string url = null;
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "username", _webhookName },
                { "avatar_url", _webhookAvatarUrl},
                { "content", message },
                { "embeds", JsonConvert.SerializeObject(new List<Embed> { embed }) },
                { "filename", fileName },
                { "fileformat", fileFormat },
                { "file", new FileParameter(File.ReadAllBytes(filePath), fileName, "application/msexcel") }
            };

            HttpWebResponse webResponse = MultipartFormDataPost("Test", postParameters);

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            webResponse.Close();

            JObject obj = JObject.Parse(fullResponse);
            JArray jArray = (JArray)obj["attachments"];
            if (jArray.Count > 0)
                url = jArray[0]["url"].ToString();
            else
                url = "empty";
            return url;
        }

        /// <summary>
        /// Function used to post plain text messages to discord
        /// </summary>
        /// <returns></returns>
        public bool PostMessage(string message)
        {
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "username", _webhookName },
                { "avatar_url", _webhookAvatarUrl},
                { "content", message }
            };

            HttpWebResponse webResponse = MultipartFormDataPost("Test", postParameters);
            webResponse.Close();

            return webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Function used to post embeded text messages to discord
        /// </summary>
        /// <returns></returns>
        public bool PostEmbededMessage(Embed embed)
        {
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "username", _webhookName },
                { "avatar_url", _webhookAvatarUrl},
                { "embeds", JsonConvert.SerializeObject(new List<Embed> { embed }) }
            };

            HttpWebResponse webResponse = MultipartFormDataPost("Test", postParameters);
            webResponse.Close();
            return webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Function used to post files to discord
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileFormat"></param>
        /// <param name="filePath"></param>
        /// <returns>A <see cref="string"></see> url to the file.</returns>
        public string PostFile(string fileName, string fileFormat, string filePath)
        {
            string url = null;
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "username", _webhookName },
                { "avatar_url", _webhookAvatarUrl},
                { "filename", fileName },
                { "fileformat", fileFormat },
                { "file", new FileParameter(File.ReadAllBytes(filePath), fileName, "application/msexcel") }
            };

            HttpWebResponse webResponse = MultipartFormDataPost("Test", postParameters);

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            webResponse.Close();

            JObject obj = JObject.Parse(fullResponse);
            JArray jArray = (JArray)obj["attachments"];
            if (jArray.Count > 0)
                url = jArray[0]["url"].ToString();
            return url;
        }

        /// <summary>
        /// Function used to convert stuff for posting the request
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="postParameters"></param>
        /// <returns><see cref="HttpWebResponse"></see></returns>
        private HttpWebResponse MultipartFormDataPost(string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());

            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(_webhookUrl, userAgent, contentType, formData);
        }

        /// <summary>
        /// Send a POST request to discord
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="userAgent"></param>
        /// <param name="contentType"></param>
        /// <param name="formData"></param>
        /// <returns><see cref="HttpWebRequest"></see></returns>
        /// <exception cref="NullReferenceException"></exception>
        private HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Gets the multipart form data using parameters and boundary
        /// </summary>
        /// <param name="postParameters"></param>
        /// <param name="boundary"></param>
        /// <returns><see cref="byte[]"></see></returns>
        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(_encoding.GetBytes("\r\n"), 0, _encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(_encoding.GetBytes(header), 0, _encoding.GetByteCount(header));
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(_encoding.GetBytes(postData), 0, _encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(_encoding.GetBytes(footer), 0, _encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}
