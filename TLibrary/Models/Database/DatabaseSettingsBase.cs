using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Database
{
    /// <inheritdoc/>
    public abstract class DatabaseSettingsBase : IDatabaseSettings
    {
        /// <inheritdoc/>
        [JsonProperty(Order = 0)]
        public string Host { get; set; } = "127.0.0.1";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 1)]
        public int Port { get; set; } = 3306;
        
        /// <inheritdoc/>
        [JsonProperty(Order = 2)]
        public string DatabaseName { get; set; } = "unturned";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 3)]
        public string UserName { get; set; } = "root";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 4)]
        public string UserPassword { get; set; } = "ascent";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 5)]
        public int TimeOut { get; set; } = 120;
    }
}
