using MySql.Data.MySqlClient;
using System;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TLibrary.Managers
{
    public abstract class DatabaseManagerBase : IDatabaseManager
    {
        public IPlugin _plugin { get; }
        public IConfigurationBase _configuration { get; }

        public DatabaseManagerBase(IPlugin plugin, IConfigurationBase configuration)
        {
            _plugin = plugin;
            _configuration = configuration;
            new I18N.West.CP1250();
            CheckSchema();
        }

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

        protected virtual void CheckSchema()
        {
            
        }
    }
}
