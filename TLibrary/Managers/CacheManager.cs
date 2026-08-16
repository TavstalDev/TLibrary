using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Database;

namespace Tavstal.TLibrary.Managers
{
    /// <inheritdoc/>
    public class CacheManager : ICacheManager
    {
        public ConcurrentDictionary<string, ConcurrentDictionary<object, object>> Cache { get; } = new  ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();

        /// <inheritdoc/>
        public void Add(string table, object key, object value)
        {
            if (!Cache.TryGetValue(table, out ConcurrentDictionary<object, object> cache))
            {
                Cache.TryAdd(table, new ConcurrentDictionary<object, object>()
                {
                    [key] = value
                });
                return;
            }
            cache.TryAdd(key, value);
        }

        /// <inheritdoc/>
        public void Update(string table, object key, object newValue)
        {
            if (!Cache.TryGetValue(table, out ConcurrentDictionary<object, object> cache))
            {
                Cache.TryAdd(table, new ConcurrentDictionary<object, object>()
                {
                    [key] = newValue
                });
                return;
            }

            if (!cache.TryGetValue(key, out var oldValue))
            {
                cache.TryAdd(key, newValue);
                return;
            }

            cache.TryUpdate(key, newValue, oldValue);
        }

        /// <inheritdoc/>
        public void RemoveTable(string table)
        {
            Cache.TryRemove(table, out _);
        }

        /// <inheritdoc/>
        public void Remove(string table, object key)
        {
            if (!Cache.TryGetValue(table, out ConcurrentDictionary<object, object> cache))
                return;
            cache.TryRemove(key, out _);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Cache.Clear();
        }

        /// <inheritdoc/>
        public T? Get<T>(string table, object key) where T : class
        {
            if (!Cache.TryGetValue(table, out var cache))
                return null;

            if (!cache.TryGetValue(key, out var value))
                return null;

            if (value is T typedValue)
                return typedValue;
            
            var actualType = value?.GetType().FullName ?? "null";
            LoggerHelper.LogError($"Failed to convert cache value from {actualType} to {typeof(T).FullName} for key '{key}' in table '{table}'.");
            return null;
        }
    }
}