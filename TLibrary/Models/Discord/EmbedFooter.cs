using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    public class EmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }

        public EmbedFooter(string text, string iconUrl, string proxyIconUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }
    }
}
