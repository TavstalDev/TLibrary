using System.Threading.Tasks;
using MySqlConnector;

namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// Defines the basic structure for a database manager.
    /// Every database manager should implement this interface.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// The configuration of the plugin.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        DatabaseSettingsBase _configuration { get; }

        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns>A new MySQL connection, or null if it fails.</returns>
        MySqlConnection? CreateConnection();

        /// <summary>
        /// Checks the database schema and creates missing tables or columns.
        /// </summary>
        void CheckSchema();
        
        /// <summary>
        /// Checks the database schema and creates missing tables or columns.
        /// </summary>
        Task CheckSchemaAsync();
    }
}
