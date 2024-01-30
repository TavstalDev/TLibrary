using Newtonsoft.Json;

namespace Tavstal.TLibrary.Compatibility.Models.Discord
{
    public class EmbedAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }

        public EmbedAuthor(string name, string url, string iconUrl, string proxyIconUrl)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }
    }
}
