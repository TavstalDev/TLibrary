using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Stores the configuration for a Discord webhook.
    /// </summary>
    public class WebhookSettings
    {
        /// <summary>
        /// The URL of the Discord webhook.
        /// </summary>
        [JsonProperty("WebhookUrl")]
        public string WebhookUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// The name the webhook will display.
        /// </summary>
        [JsonProperty("Name")]
        public string? Name { get; set; }
        
        /// <summary>
        /// The avatar image URL for the webhook.
        /// </summary>
        [JsonProperty("AvatarUrl")]
        public string? AvatarUrl { get; set; }
        
        /// <summary>
        /// The color used for Discord embeds (e.g. "#ff0000").
        /// </summary>
        [JsonProperty("Color")]
        public string? Color { get; set; }
    }
}
