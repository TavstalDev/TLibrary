using MySql.Data.MySqlClient;
using System;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TLibrary.Managers
{
    /// <summary>
    /// Base class for database managers.
    /// </summary>
    public abstract class DatabaseManagerBase : IDatabaseManager
    {
        public IPlugin _plugin { get; }
        public IConfigurationBase _configuration { get; }

        protected DatabaseManagerBase(IPlugin plugin, IConfigurationBase configuration)
        {
            _plugin = plugin;
            _configuration = configuration;
            var _ = new I18N.West.CP1250();
            CheckSchema();
        }

        /// <summary>
        /// Creates a MySqlConnection object for connecting to the database.
        /// </summary>
        /// <returns>A MySqlConnection object.</returns>
        public MySqlConnection CreateConnection()
        {
            MySqlConnection mySqlConnection = null;
            try
            {
                mySqlConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};DEFAULT COMMAND TIMEOUT={5};CharSet=utf8;",
                _configuration.GetValue<string>("Database:Host"),
                _configuration.GetValue<string>("Database:DatabaseName"),
                _configuration.GetValue<string>("Database:UserName"),
                _configuration.GetValue<string>("Database:UserPassword"),
                _configuration.GetValue<int>("Database:Port"),
                _configuration.GetValue<int>("Database:TimeOut")));
            }
            catch (Exception ex)
            {
                _plugin.GetLogger().LogException(ex.ToString());
            }
            return mySqlConnection;
        }

        /// <summary>
        /// Checks the database schema and performs necessary updates if required.
        /// </summary>
        protected virtual void CheckSchema()
        {
            
        }
    }
}
