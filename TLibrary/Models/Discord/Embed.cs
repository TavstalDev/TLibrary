using System;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    /// <summary>
    /// Represents a Discord embed message.
    /// </summary>
    public struct Embed
    {
        /// <summary>
        /// The title of the embed.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        
        /// <summary>
        /// The type of the embed (e.g. "rich").
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        
        /// <summary>
        /// The description text of the embed.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// The URL the title should link to.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// The timestamp shown in the embed.
        /// </summary>
        [JsonProperty("timestamp")]
        public  DateTime TimeStamp { get; set; }
        
        /// <summary>
        /// The color of the embed sidebar as a decimal integer.
        /// </summary>
        [JsonProperty("color")]
        public int Color { get; set; }
        
        /// <summary>
        /// The footer of the embed.
        /// </summary>
        [JsonProperty("footer")]
        public EmbedFooter Footer { get; set; }
        
        /// <summary>
        /// The image displayed in the embed.
        /// </summary>
        [JsonProperty("image")]
        public EmbedImage Image { get; set; }
        
        /// <summary>
        /// The thumbnail image of the embed.
        /// </summary>
        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }
        
        /// <summary>
        /// The video displayed in the embed.
        /// </summary>
        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }
        
        /// <summary>
        /// The author section of the embed.
        /// </summary>
        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }
        
        /// <summary>
        /// The fields displayed in the embed.
        /// </summary>
        [JsonProperty("fields")]
        public EmbedField[] Fields { get; set; }

        /// <summary>
        /// Returns the timestamp as an ISO 8601 string (sortable format).
        /// </summary>
        /// <returns>A string like "2024-01-01T12:00:00".</returns>
        public string GetIsoDateTime()
        {
            return TimeStamp.ToString("s");
        }

        /// <summary>
        /// Serializes this embed to a JSON string.
        /// </summary>
        /// <returns>The JSON representation of the embed.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
