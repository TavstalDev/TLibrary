using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    public class EmbedThumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }

        public EmbedThumbnail(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }
    }
}
