using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using Tavstal.TLibrary.Models.Config;
using Tavstal.TLibrary.Models.Database;

namespace Tavstal.TLibrary.Managers
{
    public class RedisManager : ICacheManager
    {
        private readonly IRedisConfig _config;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        
        public RedisManager(IRedisConfig config)
        {
            _config = config;
            _redis = ConnectionMultiplexer.Connect($"{_config.Host}:{_config.Port},ssl={_config.UseSsl},user={_config.UserName},password={_config.UserPassword}");
            _db = _redis.GetDatabase();
        }

        private string GetRedisKey(string table, object key) => $"{_config.Prefix}:{table}:{key}";

        public async Task AddAsync(string table, object key, object value)
        {
            var redisKey = GetRedisKey(table, key);
            var jsonValue = JsonConvert.SerializeObject(value);
            await _db.StringSetAsync(redisKey, jsonValue);
        }

        public async Task UpdateAsync(string table, object key, object newValue) =>
            await AddAsync(table, key, newValue);

        public async Task RemoveAsync(string table, object key)
        {
            var redisKey = GetRedisKey(table, key);
            await _db.KeyDeleteAsync(redisKey);
        }

        public async Task<T?> GetAsync<T>(string table, object key) where T : class
        {
            var redisKey = GetRedisKey(table, key);
            var jsonValue = await _db.StringGetAsync(redisKey);
        
            if (jsonValue.IsNullOrEmpty)
                return null;
            return JsonConvert.DeserializeObject<T>(jsonValue!);
        }

        public async Task RemoveTableAsync(string table)
        {
            foreach (var endpoint in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(endpoint);
                foreach (var key in server.Keys(pattern: $"{_config.Prefix}:{table}:*"))
                    await _db.KeyDeleteAsync(key);
            }
        }

        public async Task ClearAsync()
        {
            foreach (var endpoint in _redis.GetEndPoints())
            {
                var server = _redis.GetServer(endpoint);
                foreach (var key in server.Keys(pattern: $"{_config.Prefix}:*"))
                    await _db.KeyDeleteAsync(key);
            }
        }

        private async Task<ConnectionMultiplexer> CreateConnectionAsync()
        {
            var muxer = await ConnectionMultiplexer.ConnectAsync($"{_config.Host}:{_config.Port},ssl={_config.UseSsl},user={_config.UserName},password={_config.UserPassword}");
            return muxer;
        }
    }
}