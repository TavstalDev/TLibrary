using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    public class EmbedField
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("inline")]
        public bool? Inline { get; set; }

        public EmbedField(string name, string value, bool? inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
    }
}
