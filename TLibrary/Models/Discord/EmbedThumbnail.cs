using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents a thumbnail image in a Discord embed.
    /// </summary>
    public struct EmbedThumbnail
    {
        /// <summary>
        /// The direct URL of the thumbnail image.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// A proxied version of the thumbnail URL provided by Discord.
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        
        /// <summary>
        /// The height of the thumbnail in pixels.
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }
        
        /// <summary>
        /// The width of the thumbnail in pixels.
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }
    }
}
