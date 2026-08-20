using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Tavstal.TLibrary.Models.Config
{
    /// <inheritdoc/>
    public class RedisConfigBase : IRedisConfig
    {
        /// <inheritdoc/>
        [JsonProperty(Order = 0), YamlMember(Order = 0, Description = "Use redis cache instead of in memory cache.")]
        public bool Enable { get; set; }

        /// <inheritdoc/>
        [JsonProperty(Order = 1), YamlMember(Order = 0, Description = "Redis host, default is 'localhost'.")]
        public string Host { get; set; } = "localhost";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 2), YamlMember(Order = 1, Description = "Redis port, default is 6379.")]
        public int Port { get; set; } = 6379;
        
        /// <inheritdoc/>
        [JsonProperty(Order = 3), YamlMember(Order = 1, Description = "Redis port, default is 6379.")]
        public string Prefix { get; set; } = "tlib";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 4),  YamlMember(Order = 3,  Description = "Redis username, default is 'unturned'.")]
        public string UserName { get; set; } = "root";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 5), YamlMember(Order = 4, Description = "Redis password, default is 'ascent'.")]
        public string UserPassword { get; set; } = "ascent";
        
        /// <inheritdoc/>
        [JsonProperty(Order = 6), YamlMember(Order = 4, Description = "Use ssl, default is 'false'.")]
        public bool UseSsl { get; set; }
    }
}