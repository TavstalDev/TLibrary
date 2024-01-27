using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Compatibility;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Web.UI.WebControls;
using Tavstal.TLibrary.Compatibility.Models.Discord;
using Newtonsoft.Json;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Extensions;

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

        #region Sync
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
            return Post(message, embed, fileName, fileFormat, File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// Function used to post mixed messages to discord
        /// </summary>
        /// <param name="message"></param>
        /// <param name="embed"></param>
        /// <param name="fileName"></param>
        /// <param name="fileFormat"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public string Post(string message, Embed embed, string fileName, string fileFormat, byte[] fileData)
        {
            string url = null;
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "payload_json", new JsonParameter(new { username = _webhookName, avatar_url = _webhookAvatarUrl, content = message, embeds = new List<object> { embed } }, "application/json") },
                { "filename", fileName },
                { "fileformat", fileFormat },
                { "file", new FileParameter(fileData, fileName, "application/msexcel") }
            };

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = MultipartFormDataPost("Test", postParameters);
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(reader.ReadToEnd());
                }
                return null;
            }

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
                { "content", message }
            };
            if (!_webhookName.IsNullOrEmpty())
                postParameters.Add("username", _webhookName);
            if (!_webhookAvatarUrl.IsNullOrEmpty())
                postParameters.Add("avatar_url", _webhookAvatarUrl);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = MultipartFormDataPost("Test", postParameters);
                webResponse.Close();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(reader.ReadToEnd());
                }
                return false;
            }
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
                { "embeds", new List<object> { embed } }
            };
            if (!_webhookName.IsNullOrEmpty())
                postParameters.Add("username", _webhookName);
            if (!_webhookAvatarUrl.IsNullOrEmpty())
                postParameters.Add("avatar_url", _webhookAvatarUrl);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = JsonPost("Test", postParameters);
                webResponse.Close();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(reader.ReadToEnd());
                }
                return false;
            }
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
                { "filename", fileName },
                { "fileformat", fileFormat },
                { "file", new FileParameter(File.ReadAllBytes(filePath), fileName, "application/msexcel") }
            };
            if (!_webhookName.IsNullOrEmpty())
                postParameters.Add("username", _webhookName);
            if (!_webhookAvatarUrl.IsNullOrEmpty())
                postParameters.Add("avatar_url", _webhookAvatarUrl);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = MultipartFormDataPost("Test", postParameters);
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(reader.ReadToEnd());
                }
                return null;
            }

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

        private HttpWebResponse JsonPost(string userAgent, Dictionary<string, object> postParameters)
        {
            HttpWebRequest request = WebRequest.Create(_webhookUrl) as HttpWebRequest;
            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            byte[] formData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postParameters));

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = "application/json";
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

                if (param.Value is FileParameter fileToUpload)
                {
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(_encoding.GetBytes(header), 0, _encoding.GetByteCount(header));
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else if (param.Value is JsonParameter jsonParam)
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\nContent-Type: {3}\r\n\r\n{2}\r\n\r\n",
                        boundary,
                        param.Key,
                        jsonParam.Content,
                        jsonParam.ContentType ?? "application/json");
                    formDataStream.Write(_encoding.GetBytes(postData), 0, _encoding.GetByteCount(postData));
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
        #endregion

        #region Async
        /// <summary>
        /// Function used to post mixed messages to discord
        /// </summary>
        /// <param name="message"></param>
        /// <param name="embed"></param>
        /// <param name="fileName"></param>
        /// <param name="fileFormat"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(string message, Embed embed, string fileName, string fileFormat, string filePath)
        {
            return await PostAsync(message, embed, fileName, fileFormat, File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// Function used to post mixed messages to discord
        /// </summary>
        /// <param name="message"></param>
        /// <param name="embed"></param>
        /// <param name="fileName"></param>
        /// <param name="fileFormat"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(string message, Embed embed, string fileName, string fileFormat, byte[] fileData)
        {
            string url = null;
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "payload_json", new JsonParameter(new { username = _webhookName, avatar_url = _webhookAvatarUrl, content = message, embeds = new List<object> { embed } }, "application/json") },
                { "filename", fileName },
                { "fileformat", fileFormat },
                { "file", new FileParameter(fileData, fileName, "application/msexcel") }
            };

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = await MultipartFormDataPostAsync("Test", postParameters);
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(await reader.ReadToEndAsync());
                }
                return null;
            }

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = await responseReader.ReadToEndAsync();
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
        public async Task<bool> PostMessageAsync(string message)
        {
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "content", message }
            };
            if (!_webhookName.IsNullOrEmpty())
                postParameters.Add("username", _webhookName);
            if (!_webhookAvatarUrl.IsNullOrEmpty())
                postParameters.Add("avatar_url", _webhookAvatarUrl);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = await MultipartFormDataPostAsync("Test", postParameters);
                webResponse.Close();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(await reader.ReadToEndAsync());
                }
                return false;
            }
            return webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Function used to post embeded text messages to discord
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PostEmbededMessageAsync(Embed embed)
        {
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "embeds", new List<object> { embed } }
            };
            if (!_webhookName.IsNullOrEmpty())
                postParameters.Add("username", _webhookName);
            if (!_webhookAvatarUrl.IsNullOrEmpty())
                postParameters.Add("avatar_url", _webhookAvatarUrl);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = await JsonPostAsync("Test", postParameters);
                webResponse.Close();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(await reader.ReadToEndAsync());
                }
                return false;
            }
            return webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Function used to post files to discord
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileFormat"></param>
        /// <param name="filePath"></param>
        /// <returns>A <see cref="string"></see> url to the file.</returns>
        public async Task<string> PostFileAsync(string fileName, string fileFormat, string filePath)
        {
            string url = null;
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "filename", fileName },
                { "fileformat", fileFormat },
                { "file", new FileParameter(File.ReadAllBytes(filePath), fileName, "application/msexcel") }
            };
            if (!_webhookName.IsNullOrEmpty())
                postParameters.Add("username", _webhookName);
            if (!_webhookAvatarUrl.IsNullOrEmpty())
                postParameters.Add("avatar_url", _webhookAvatarUrl);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = await MultipartFormDataPostAsync("Test", postParameters);
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    LoggerHelper.LogError(await reader.ReadToEndAsync());
                }
                return null;
            }

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = await responseReader.ReadToEndAsync();
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
        private async Task<HttpWebResponse> MultipartFormDataPostAsync(string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());

            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = await GetMultipartFormDataAsync(postParameters, formDataBoundary);
            return await PostFormAsync(_webhookUrl, userAgent, contentType, formData);
        }

        private async Task<HttpWebResponse> JsonPostAsync(string userAgent, Dictionary<string, object> postParameters)
        {
            HttpWebRequest request = WebRequest.Create(_webhookUrl) as HttpWebRequest;
            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            byte[] formData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postParameters));

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return await request.GetResponseAsync() as HttpWebResponse;
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
        private async Task<HttpWebResponse> PostFormAsync(string postUrl, string userAgent, string contentType, byte[] formData)
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

            return await request.GetResponseAsync() as HttpWebResponse;
        }

        /// <summary>
        /// Gets the multipart form data using parameters and boundary
        /// </summary>
        /// <param name="postParameters"></param>
        /// <param name="boundary"></param>
        /// <returns><see cref="byte[]"></see></returns>
        private async Task<byte[]> GetMultipartFormDataAsync(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(_encoding.GetBytes("\r\n"), 0, _encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter fileToUpload)
                {
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(_encoding.GetBytes(header), 0, _encoding.GetByteCount(header));
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else if (param.Value is JsonParameter jsonParam)
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\nContent-Type: {3}\r\n\r\n{2}\r\n\r\n",
                        boundary,
                        param.Key,
                        jsonParam.Content,
                        jsonParam.ContentType ?? "application/json");
                    formDataStream.Write(_encoding.GetBytes(postData), 0, _encoding.GetByteCount(postData));
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
            await formDataStream.ReadAsync(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
        #endregion
    }
}
