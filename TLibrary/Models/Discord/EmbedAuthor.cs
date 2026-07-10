using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents the author of a Discord embed.
    /// </summary>
    public struct EmbedAuthor
    {
        /// <summary>
        /// The display name of the author.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The URL of the author (opens when clicking the name).
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// The URL of the author's icon image.
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        
        /// <summary>
        /// A proxied version of the icon URL provided by Discord.
        /// </summary>
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}
