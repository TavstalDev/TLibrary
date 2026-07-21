using Newtonsoft.Json;
using Tavstal.TLibrary.Models.Logging;
using YamlDotNet.Serialization;

namespace Tavstal.TLibrary.Models.Config
{
    public class GeneralConfig
    {
        [JsonProperty(Order = 0), YamlMember(Order = 0, Description = "Enable tools and messages used for development.")]
        public bool DebugMode { get; set; }
        
        [JsonProperty(Order = 1), YamlMember(Order = 1, Description = "Log level for the plugin, default is 'INFO'.\nValues: DEBUG, INFO, WARNING, ERROR, COMMAND")]
        public ELogLevel LogLevel { get; set; } =  ELogLevel.INFO;
        
        [JsonProperty(Order = 2), YamlMember(Order = 2, Description = "Locale used for the plugin, default is 'en'.")] 
        public string Locale { get; set; } = "en";

        [JsonProperty(Order = 3), YamlMember(Order = 3, Description = "Should the plugin download locale packs from the server?")]
        public bool DownloadLocalePacks { get; set; } = true;
        
        [JsonProperty(Order = 4), YamlMember(Order = 4, Description = "Icon to display in chat messages. Set it to empty or null to disable.")]
        public string? MessageIcon { get; set; }
    }
}