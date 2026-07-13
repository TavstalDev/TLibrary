using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Models.Discord
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
        /// <summary>
        /// The custom display name of the webhook.
        /// </summary>
        private readonly string? _webhookName;
        /// <summary>
        /// The custom avatar image URL of the webhook.
        /// </summary>
        private readonly string? _webhookAvatarUrl;
        /// <summary>
        /// The bitness of the operating system (32 or 64).
        /// </summary>
        private static int OperatingSystemBitness => Environment.Is64BitOperatingSystem ? 64 : 32;
        /// <summary>
        /// The user agent string used in HTTP requests.
        /// </summary>
        private static readonly string _userAgent = $"TLibrary (lang=DOTNET;v={Environment.Version};bit={OperatingSystemBitness};os={Environment.OSVersion};)";

        /// <summary>
        /// Creates a Discord webhook with only the webhook URL.
        /// </summary>
        /// <param name="url">The Discord webhook URL.</param>
        public DiscordWebhook(string url)
        {
            _webhookUrl = url;
        }

        /// <summary>
        /// Creates a Discord webhook with a custom name and URL.
        /// </summary>
        /// <param name="name">The name the webhook will display.</param>
        /// <param name="url">The Discord webhook URL.</param>
        public DiscordWebhook(string name, string url)
        {
            _webhookName = name;
            _webhookUrl = url;
        }

        /// <summary>
        /// Creates a Discord webhook with a custom name, avatar, and URL.
        /// </summary>
        /// <param name="name">The name the webhook will display.</param>
        /// <param name="avatar">The URL of the avatar image the webhook will use.</param>
        /// <param name="url">The Discord webhook URL.</param>
        public DiscordWebhook(string name, string avatar, string url)
        {
            _webhookName = name;
            _webhookAvatarUrl = avatar;
            _webhookUrl = url;
        }
        
        /// <summary>
        /// Sends a message with an embed and a file loaded from disk.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <param name="embed">The Discord embed to attach.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileFormat">The format of the file (e.g. "png").</param>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>The response content from Discord, or <see langword="null"/> if it failed.</returns>
        public async Task<string?> PostAsync(string message, Embed embed, string fileName, string fileFormat, string filePath) => 
            await PostAsync(message, embed, fileName, fileFormat, File.ReadAllBytes(filePath));

        /// <summary>
        /// Sends a message with an embed and a file from raw byte data.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <param name="embed">The Discord embed to attach.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileFormat">The format of the file (e.g. "png").</param>
        /// <param name="fileData">The raw byte content of the file.</param>
        /// <returns>The response content from Discord, or <see langword="null"/> if it failed.</returns>
        public async Task<string?> PostAsync(string message, Embed embed, string fileName, string fileFormat, byte[] fileData) =>
            await PostAsync(message, embed, new WebhookFile
            {
                Name = fileName,
                Format = fileFormat,
                Content = fileData
            });

        /// <summary>
        /// Sends a plain text message to Discord.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <returns><see langword="true"/> if the message was sent successfully.</returns>
        public async Task<bool> PostMessageAsync(string message) =>
            string.Equals("empty",  await PostAsync(message), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Sends an embed message to Discord.
        /// </summary>
        /// <param name="embed">The Discord embed to send.</param>
        /// <returns><see langword="true"/> if the message was sent successfully.</returns>
        public async Task<bool> PostEmbededMessageAsync(Embed embed) =>
            string.Equals("empty",  await PostAsync(null, embed), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Sends a file to Discord and returns its URL.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileFormat">The format of the file (e.g. "png").</param>
        /// <param name="filePath">The full path to the file on disk.</param>
        /// <returns>The URL of the uploaded file, or <see langword="null"/> if it failed.</returns>
        public async Task<string?> PostFileAsync(string fileName, string fileFormat, string filePath) =>
            await PostAsync(null, null, new WebhookFile {
                Name = fileName,
                Format = fileFormat,
                Content = File.ReadAllBytes(filePath)
            });
        
        /// <summary>
        /// Sends a message to the Discord webhook with an optional embed and file.
        /// </summary>
        /// <param name="message">The text message to send (optional).</param>
        /// <param name="embed">A Discord embed to attach (optional).</param>
        /// <param name="file">A file to upload with the message (optional).</param>
        /// <returns>The response content from Discord, or <see langword="null"/> if the request failed.</returns>
        public async Task<string?> PostAsync(string? message = null, Embed? embed = null, WebhookFile? file = null)
        {
            JsonParameter payload;
            if (embed != null)
                payload = new JsonParameter
                {
                    Content = JsonConvert.SerializeObject(new {
                        username = _webhookName,
                        avatar_url = _webhookAvatarUrl,
                        content = message,
                        embeds = new List<object> { embed }
                    }),
                    ContentType = "application/json"
                };
            else 
                payload = new JsonParameter
                {
                    Content = JsonConvert.SerializeObject(new {
                        username = _webhookName,
                        avatar_url = _webhookAvatarUrl,
                        content = message
                    }),
                    ContentType = "application/json"
                };
            
            Dictionary<string, object> postParameters = new Dictionary<string, object>
            {
                { "payload_json", payload },
            };
            if (file != null)
            {
                postParameters.Add("filename", file.Value.Name);
                postParameters.Add("fileformat", file.Value.Format);
                postParameters.Add("file", new FileParameter
                {
                    File = file.Value.Content,
                    FileName =  file.Value.Name,
                    ContentType = "application/octet-stream"
                });
            }

            HttpWebResponse? webResponse;
            try
            {
                webResponse = await WebhookHelper.MultipartFormDataPostAsync(_webhookUrl, _userAgent, postParameters);
            }
            catch (WebException ex)
            {
                using var stream = ex.Response.GetResponseStream();
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    LoggerHelper.LogError(await reader.ReadToEndAsync());
                }

                return null;
            }

            // ReSharper disable once UseNullPropagation
            if (webResponse == null)
                return null;
            
            var responseStream = webResponse.GetResponseStream();
            if (responseStream == null)
                return null;
            
            using StreamReader responseReader = new StreamReader(responseStream);
            string fullResponse =  await responseReader.ReadToEndAsync();
            webResponse.Close();

            JObject obj = JObject.Parse(fullResponse);
            string? url;
            if (obj.TryGetValue("attachments", out var attachments) && attachments is JArray attachmentArray)
                url = attachmentArray[0]["url"]?.ToString();
            else 
                url = "empty";
            return url;
        }
    }
}
