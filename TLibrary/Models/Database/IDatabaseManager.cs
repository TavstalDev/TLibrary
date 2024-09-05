using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// Base interface for all database managers.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// The configuration of the plugin.
        /// </summary>
        IConfigurationBase _configuration { get; }

        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns></returns>
        MySqlConnection CreateConnection();

        void CheckSchema();
        
        Task CheckSchemaAsync();
    }
}
