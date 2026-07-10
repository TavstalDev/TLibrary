using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents a field inside a Discord embed.
    /// </summary>
    public struct EmbedField
    {
        /// <summary>
        /// The name or title of the field.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The value or content of the field.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        
        /// <summary>
        /// If <see langword="true"/>, the field is displayed inline next to other inline fields.
        /// </summary>
        [JsonProperty("inline")]
        public bool? Inline { get; set; }
    }
}
