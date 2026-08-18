using Tavstal.TLibrary.Models.Database;

namespace Tavstal.TLibrary.Managers
{
    public class RedisManager : ICacheManager
    {
        // TODO
        public void Add(string table, object key, object value)
        {
            throw new System.NotImplementedException();
        }

        public void Update(string table, object key, object newValue)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveTable(string table)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(string table, object key)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public T? Get<T>(string table, object key) where T : class
        {
            throw new System.NotImplementedException();
        }
    }
}