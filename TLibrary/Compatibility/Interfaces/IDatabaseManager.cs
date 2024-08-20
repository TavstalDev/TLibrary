using MySql.Data.MySqlClient;

namespace Tavstal.TLibrary.Compatibility.Interfaces
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
    }
}
