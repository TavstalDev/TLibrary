using System.Collections.Concurrent;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Database;

namespace Tavstal.TLibrary.Managers
{
    /// <inheritdoc/>
    public class CacheManager : ICacheManager
    {
        public ConcurrentDictionary<string, ConcurrentDictionary<object, object>> Cache { get; } = new  ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();

        /// <inheritdoc/>
        public Task AddAsync(string table, object key, object value)
        {
            if (!Cache.TryGetValue(table, out ConcurrentDictionary<object, object> cache))
            {
                Cache.TryAdd(table, new ConcurrentDictionary<object, object>()
                {
                    [key] = value
                });
                return Task.CompletedTask;
            }
            cache.TryAdd(key, value);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task UpdateAsync(string table, object key, object newValue)
        {
            if (!Cache.TryGetValue(table, out ConcurrentDictionary<object, object> cache))
            {
                Cache.TryAdd(table, new ConcurrentDictionary<object, object>()
                {
                    [key] = newValue
                });
                return Task.CompletedTask;
            }

            if (!cache.TryGetValue(key, out var oldValue))
            {
                cache.TryAdd(key, newValue);
                return Task.CompletedTask;
            }

            cache.TryUpdate(key, newValue, oldValue);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveTableAsync(string table)
        {
            Cache.TryRemove(table, out _);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RemoveAsync(string table, object key)
        {
            if (!Cache.TryGetValue(table, out ConcurrentDictionary<object, object> cache))
                return Task.CompletedTask;
            cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ClearAsync()
        {
            Cache.Clear();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<T?> GetAsync<T>(string table, object key) where T : class
        {
            if (!Cache.TryGetValue(table, out var cache))
                return Task.FromResult<T?>(null);

            if (!cache.TryGetValue(key, out var value))
                return Task.FromResult<T?>(null);

            if (value is T typedValue)
                return Task.FromResult<T?>(typedValue);
            
            var actualType = value?.GetType().FullName ?? "null";
            LoggerHelper.LogError($"Failed to convert cache value from {actualType} to {typeof(T).FullName} for key '{key}' in table '{table}'.");
            return Task.FromResult<T?>(null);
        }
    }
}