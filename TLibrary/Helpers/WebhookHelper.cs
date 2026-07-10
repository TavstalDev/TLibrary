using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Models.Discord;

namespace Tavstal.TLibrary.Helpers
{
    /// <summary>
    /// Provides helper methods for sending multipart form data to webhooks.
    /// </summary>
    public static class WebhookHelper
    {
        /// <summary>
        /// Sends a multipart form data POST request to a webhook URL.
        /// </summary>
        /// <param name="webhookUrl">The target webhook URL.</param>
        /// <param name="userAgent">The user agent string to use in the request.</param>
        /// <param name="postParameters">The parameters to send in the request body.</param>
        /// <returns>The HTTP response from the webhook, or <see langword="null"/> if it failed.</returns>
        public static async Task<HttpWebResponse?> MultipartFormDataPostAsync(string webhookUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = $"----------{Guid.NewGuid():N}";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = await GetMultipartFormDataAsync(postParameters, formDataBoundary);
            return await PostFormAsync(webhookUrl, userAgent, contentType, formData);
        }
        
        /// <summary>
        /// Builds the raw byte data for a multipart form request body.
        /// </summary>
        /// <param name="postParameters">The parameters to include in the form.</param>
        /// <param name="boundary">The boundary string used to separate parts.</param>
        /// <returns>The form data as a byte array.</returns>
        public static async Task<byte[]> GetMultipartFormDataAsync(Dictionary<string, object> postParameters, string boundary)
        {
            using var formDataStream = new MemoryStream();
            byte[] crlf = Encoding.UTF8.GetBytes("\r\n");
            bool needsClrf = false;

            foreach (var chunk in PrepareChunks(postParameters, boundary))
            {
                if (needsClrf) await formDataStream.WriteAsync(crlf, 0, crlf.Length);
                needsClrf = true;

                await formDataStream.WriteAsync(chunk.Header, 0, chunk.Header.Length);
                if (chunk.HasSeparatePayload)
                    await formDataStream.WriteAsync(chunk.Payload, 0, chunk.Payload.Length);
            }

            byte[] footer = Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n");
            await formDataStream.WriteAsync(footer, 0, footer.Length);
            return formDataStream.ToArray();
        }
        
        /// <summary>
        /// Sends a POST request with the prepared form data.
        /// </summary>
        /// <param name="postUrl">The target URL.</param>
        /// <param name="userAgent">The user agent string.</param>
        /// <param name="contentType">The content type header value.</param>
        /// <param name="formData">The raw form data to send.</param>
        /// <returns>The HTTP response, or <see langword="null"/> if it failed.</returns>
        private static async Task<HttpWebResponse?> PostFormAsync(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            var request = PrepareWebRequest(postUrl, userAgent, contentType, formData);
            using (Stream requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(formData, 0, formData.Length);
                requestStream.Close();
            }
            return await request.GetResponseAsync() as HttpWebResponse;
        }

        /// <summary>
        /// Creates and configures an <see cref="HttpWebRequest"/> for a POST request.
        /// </summary>
        /// <param name="postUrl">The target URL.</param>
        /// <param name="userAgent">The user agent string.</param>
        /// <param name="contentType">The content type header value.</param>
        /// <param name="formData">The form data to set the content length from.</param>
        /// <returns>The configured <see cref="HttpWebRequest"/>.</returns>
        private static HttpWebRequest PrepareWebRequest(string postUrl, string userAgent, string contentType,
            byte[] formData)
        {
            if (!(WebRequest.Create(postUrl) is HttpWebRequest request))
                throw new NullReferenceException("request is not a http request");

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;
            return request;
        }
        
        /// <summary>
        /// Splits the post parameters into multipart chunks ready for writing.
        /// </summary>
        /// <param name="postParameters">The parameters to split.</param>
        /// <param name="boundary">The boundary string for the multipart form.</param>
        /// <returns>A collection of <see cref="MultipartChunk"/> objects.</returns>
        private static IEnumerable<MultipartChunk> PrepareChunks(Dictionary<string, object> postParameters, string boundary)
        {
            foreach (var param in postParameters)
            {
                switch (param.Value)
                {
                    case FileParameter fileToUpload:
                        string fileHeader =
                            $"--{boundary}\r\nContent-Disposition: form-data; name=\"{param.Key}\"; filename=\"{fileToUpload.FileName ?? param.Key}\"\r\nContent-Type: {fileToUpload.ContentType ?? "application/octet-stream"}\r\n\r\n";
                        yield return new MultipartChunk
                        {
                            Header = Encoding.UTF8.GetBytes(fileHeader), Payload = fileToUpload.File,
                            HasSeparatePayload = true
                        };
                        break;

                    case JsonParameter jsonParam:
                        string jsonBlock =
                            $"--{boundary}\r\nContent-Disposition: form-data; name=\"{param.Key}\"\r\nContent-Type: {jsonParam.ContentType}\r\n\r\n{jsonParam.Content}";
                        yield return new MultipartChunk
                            { Header = Encoding.UTF8.GetBytes(jsonBlock), HasSeparatePayload = false };
                        break;

                    default:
                        string defaultBlock =
                            $"--{boundary}\r\nContent-Disposition: form-data; name=\"{param.Key}\"\r\n\r\n{param.Value}";
                        yield return new MultipartChunk
                            { Header = Encoding.UTF8.GetBytes(defaultBlock), HasSeparatePayload = false };
                        break;
                }
            }
        }
    }
}