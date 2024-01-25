using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers.General;
using UnityEngine;

namespace Tavstal.TLibrary.Compatibility.Models.Discord
{
    [Serializable]
    public class WebhookSettings
    {
        [JsonProperty("WebhookUrl")]
        public string WebhookUrl { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("AvatarUrl")]
        public string AvatarUrl { get; set; }
        [JsonProperty("Color")]
        public string Color { get; set; }

        public WebhookSettings() { }

        public WebhookSettings(string webhookUrl, string name = null, string avatarUrl = null, string color = null)
        {
            WebhookUrl = webhookUrl;
            Name = name;
            AvatarUrl = avatarUrl;
            Color = color;
        }
    }
}
