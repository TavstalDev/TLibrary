using Newtonsoft.Json;
using Tavstal.TLibrary.Models.Database;
using YamlDotNet.Serialization;

namespace Tavstal.TLibrary.Models.Config
{
    /// <inheritdoc/>
    public abstract class DatabaseConfigBase : IDatabaseConfig
    {
        /// <inheritdoc/>
        [JsonProperty(Order = 0), YamlMember(Order = 0, Description = "Database host, default is 'localhost'.")]
        public string Host { get; set; } = "localhost";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 1), YamlMember(Order = 1, Description = "Database port, default is 3306.")]
        public int Port { get; set; } = 3306;
        
        /// <inheritdoc/>
        [JsonProperty(Order = 2), YamlMember(Order = 2, Description = "Database name, default is 'unturned'.")]
        public string DatabaseName { get; set; } = "unturned";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 3),  YamlMember(Order = 3,  Description = "Database username, default is 'unturned'.")]
        public string UserName { get; set; } = "root";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 4), YamlMember(Order = 4, Description = "Database password, default is 'ascent'.")]
        public string UserPassword { get; set; } = "ascent";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 5), YamlMember(Order = 5, Description = "Database connection timeout in seconds, default is 120.")]
        public int TimeOut { get; set; } = 120;
    }
}
