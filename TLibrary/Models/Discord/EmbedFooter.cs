using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents the footer of a Discord embed.
    /// </summary>
    public struct EmbedFooter
    {
        /// <summary>
        /// The text displayed in the footer.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }
        
        /// <summary>
        /// The URL of the icon shown next to the footer text.
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
