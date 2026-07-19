using System;
using System.Threading.Tasks;
using MySqlConnector;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Threading;

namespace Tavstal.TLibrary.Managers
{
    /// <summary>
    /// Base class for database managers.
    /// </summary>
    public abstract class DatabaseManagerBase : IDatabaseManager
    {
        // ReSharper disable once InconsistentNaming
        public IPlugin _plugin { get; }
        public DatabaseSettingsBase _configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManagerBase"/> class.
        /// </summary>
        /// <param name="plugin">The plugin that owns this database manager.</param>
        /// <param name="configuration">The database connection settings.</param>
        protected DatabaseManagerBase(IPlugin plugin, DatabaseSettingsBase configuration)
        {
            _plugin = plugin;
            _configuration = configuration;
            // Forces Mono's compiler/linker to include the CP1250 codepage
            _ = new I18N.West.CP1250();
            // ReSharper disable once VirtualMemberCallInConstructor
            BackgroundThreadDispatcher.Run(async () => await CheckSchemaAsync());
        }

        /// <summary>
        /// Creates a MySqlConnection object for connecting to the database.
        /// </summary>
        /// <returns>A MySqlConnection object.</returns>
        public MySqlConnection? CreateConnection()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = _configuration.Host,
                Database = _configuration.DatabaseName,
                UserID = _configuration.UserName,
                Password = _configuration.UserPassword,
                Port = (uint)_configuration.Port,
                DefaultCommandTimeout = (uint)_configuration.TimeOut,
                CharacterSet = "utf8",
                Pooling = true, 
                MinimumPoolSize = 5,
                MaximumPoolSize = 10,
            };
            
            MySqlConnection? mySqlConnection = null;
            try
            {
                mySqlConnection = new MySqlConnection(builder.ConnectionString);
            }
            catch (Exception ex)
            {
                _plugin.GetLogger().Error("Failed to create MySqlConnection.", ex);
            }
            return mySqlConnection;
        }

        /// <summary>
        /// Checks the database schema and performs necessary updates if required.
        /// </summary>
        public virtual Task CheckSchemaAsync() => Task.CompletedTask;
    }
}
