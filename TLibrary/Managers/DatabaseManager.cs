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

namespace Tavstal.TLibrary.Managers
{
    public abstract class DatabaseManagerBase
    {
        private IRocketPluginConfiguration _configuration;

        public DatabaseManagerBase()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        public MySqlConnection createConnection()
        {
            MySqlConnection mySqlConnection = (MySqlConnection)null;
            try
            {
                if (_configuration.DatabaseData.Port == 0)
                {
                    config.DatabaseData.Port = 3306;
                    main.Configuration.Save();
                }
                mySqlConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};DEFAULT COMMAND TIMEOUT={5};CharSet=utf8;",
                    config.DatabaseData.Host,
                    config.DatabaseData.DatabaseName,
                    config.DatabaseData.UserName,
                    config.DatabaseData.UserPassword,
                    config.DatabaseData.Port,
                    config.DatabaseData.TimeOut));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.ToString());
            }
            return mySqlConnection;
        }

        protected void CheckSchema()
        {
            try
            {
                MySqlConnection connection = this.createConnection();
                MySqlCommand command = connection.CreateCommand();

                #region Player Account Data
                command.CommandText = "show tables like '" + config.DatabaseData.Table_playerdata + "'";
                connection.Open();
                if (command.ExecuteScalar() == null)
                {
                    command.CommandText = "CREATE TABLE " + config.DatabaseData.Table_playerdata +
                    "(id INT(8) NOT NULL AUTO_INCREMENT," +
                    "steamID VARCHAR(50) NOT NULL," +
                    "playerName TEXT NOT NULL," +
                    "phoneNumber TEXT NOT NULL," +
                    "iconUrl TEXT NOT NULL," +
                    "installedApps TEXT NOT NULL," +
                    "galleryUrls TEXT NOT NULL," +
                    "theme TEXT NOT NULL," +
                    "notification TEXT NOT NULL," +
                    "lockscreen TEXT NOT NULL," +
                    "contacts TEXT NOT NULL," +
                    "messages TEXT NOT NULL," +
                    "twitter TEXT NOT NULL," +
                    "mail TEXT NOT NULL," +
                    "instagram TEXT NOT NULL," +
                    "tinder TEXT NOT NULL," +
                    "notes TEXT NOT NULL," +
                    "darkchats TEXT NOT NULL," +
                    "transactions TEXT NOT NULL," +
                    "PRIMARY KEY(id));";
                    command.ExecuteNonQuery();
                }
                #endregion

                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex.ToString());
            }
        }
    }
}
