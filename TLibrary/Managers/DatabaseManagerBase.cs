using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Tavstal.TLibrary.Helpers.LoggerHelper;
using Rocket.API;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Compatibility.Interfaces;

namespace Tavstal.TLibrary.Managers
{
    public abstract class DatabaseManagerBase : IDatabaseManager
    {
        public IConfigurationBase _configuration { get; }

        public DatabaseManagerBase(IConfigurationBase configuration)
        {
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
                _configuration.GetValue<int>("Database:Timeout")));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.ToString());
            }
            return mySqlConnection;
        }

        public virtual void CheckSchema()
        {
            
        }
    }
}
