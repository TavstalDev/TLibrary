using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tavstal.TLibrary.Compatibility
{
    public abstract class DatabaseSettingsBase : IDatabaseSettings
    {
        [JsonProperty(Order = 0)]
        public string Host { get; set; } = "127.0.0.1";
        [JsonProperty(Order = 1)]
        public int Port { get; set; } = 3306;
        [JsonProperty(Order = 2)]
        public string DatabaseName { get; set; } = "unturned";
        [JsonProperty(Order = 3)]
        public string UserName { get; set; } = "root";
        [JsonProperty(Order = 4)]
        public string UserPassword { get; set; } = "ascent";
        [JsonProperty(Order = 5)]
        public int TimeOut { get; set; } = 120;
    }
}
