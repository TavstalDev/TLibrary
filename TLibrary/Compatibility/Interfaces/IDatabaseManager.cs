using MySql.Data.MySqlClient;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
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
