using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Compatibility.Classes;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Web.UI.WebControls;
using Tavstal.TLibrary.Compatibility;

namespace Tavstal.TLibrary.Services
{
    public class DiscordWebhook
    {
        public string Url { get; private set; }
        private static readonly Encoding encoding = Encoding.UTF8;

        public DiscordWebhook(string url)
        {
            Url = url;
        }

        public string PostFile(string fileName, string fileFormat, string filePath)
        {
            string url = null;
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("filename", fileName);
            postParameters.Add("fileformat", fileFormat);
            postParameters.Add("file", new FileParameter(File.ReadAllBytes(filePath), fileName, "application/msexcel"));

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

        public HttpWebResponse MultipartFormDataPost(string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());

            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(Url, userAgent, contentType, formData);
        }

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

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }
}
