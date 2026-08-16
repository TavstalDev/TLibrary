namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// Manages an in-memory cache organized into named tables, where each table stores key-value pairs.
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Adds a key-value pair to the specified table.
        /// </summary>
        /// <param name="table">The name of the table to add the entry to.</param>
        /// <param name="key">The key of the entry.</param>
        /// <param name="value">The value to store under the key.</param>
        void Add(string table, object key, object value);
        
        /// <summary>
        /// Replaces the value of an existing entry in the specified table.
        /// </summary>
        /// <param name="table">The name of the table containing the entry.</param>
        /// <param name="key">The key of the entry to update.</param>
        /// <param name="newValue">The new value to store under the key.</param>
        void Update(string table, object key, object newValue);
        
        /// <summary>
        /// Removes an entire table and all of its entries.
        /// </summary>
        /// <param name="table">The name of the table to remove.</param>
        void RemoveTable(string table);
        
        /// <summary>
        /// Removes a single entry from the specified table.
        /// </summary>
        /// <param name="table">The name of the table containing the entry.</param>
        /// <param name="key">The key of the entry to remove.</param>
        void Remove(string table, object key);
        
        /// <summary>
        /// Removes all tables and entries from the cache.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Gets the value stored under the specified key in the given table, cast to type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <param name="table">The name of the table containing the entry.</param>
        /// <param name="key">The key of the entry to get.</param>
        /// <returns>The value cast to <typeparamref name="T"/>, or <c>null</c> if the entry does not exist or cannot be cast.</returns>
        T? Get<T>(string table, object key) where T : class;
    }
}