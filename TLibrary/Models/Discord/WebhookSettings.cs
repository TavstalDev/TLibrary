using Newtonsoft.Json;
using YamlDotNet.Serialization;

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
        [JsonProperty("WebhookUrl", Order = 0), YamlMember(Order = 0)]
        public string WebhookUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// The name the webhook will display.
        /// </summary>
        [JsonProperty("Name", Order = 1), YamlMember(Order = 1)]
        public string? Name { get; set; }
        
        /// <summary>
        /// The avatar image URL for the webhook.
        /// </summary>
        [JsonProperty("AvatarUrl", Order = 2), YamlMember(Order = 2)]
        public string? AvatarUrl { get; set; }
        
        /// <summary>
        /// The color used for Discord embeds (e.g. "#ff0000").
        /// </summary>
        [JsonProperty("Color", Order = 3), YamlMember(Order = 3)]
        public string? Color { get; set; }
    }
}
