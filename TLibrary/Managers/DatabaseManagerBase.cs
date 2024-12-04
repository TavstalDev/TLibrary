using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Managers
{
    /// <summary>
    /// Base class for database managers.
    /// </summary>
    public abstract class DatabaseManagerBase : IDatabaseManager
    {
        // ReSharper disable once InconsistentNaming
        public IPlugin _plugin { get; }
        public IConfigurationBase _configuration { get; }

        protected DatabaseManagerBase(IPlugin plugin, IConfigurationBase configuration)
        {
            _plugin = plugin;
            _configuration = configuration;
            var _ = new I18N.West.CP1250();
            // ReSharper disable once VirtualMemberCallInConstructor
            CheckSchema();
            Task.Run(async () => await CheckSchemaAsync());
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
        public virtual void CheckSchema()
        {
            
        }

        /// <summary>
        /// Checks the database schema and performs necessary updates if required.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task CheckSchemaAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            
        }
    }
}
