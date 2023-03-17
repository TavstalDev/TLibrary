using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Tavstal.TLibrary.Helpers.LoggerHelper;

namespace Tavstal.TLibrary.Managers
{
    public class DatabaseManager
    {
        public LibraryMain main => LibraryMain.Instance;
        public PluginConfig config => LibraryMain.Instance.Configuration.Instance;

        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        public MySqlConnection createConnection()
        {
            MySqlConnection mySqlConnection = (MySqlConnection)null;
            try
            {
                if (config.DatabaseData.Port == 0)
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

        public void CheckSchema()
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

        #region PlayerAccount
        /*public List<PlayerAccount> GetPlayerAccounts()
        {
            List<PlayerAccount> numList = new List<PlayerAccount>();
            try
            {
                MySqlConnection connection = this.createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select * from `" + config.DatabaseData.Table_playerdata + "`;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //mySqlDataReader.GetOrdinal("");
                    PlayerAccount num = new PlayerAccount()
                    {
                        PlayerSteamID = reader.GetUInt64("steamID"),
                        PhoneNumber = reader.GetString("phoneNumber"),
                        PlayerLastName = reader.GetString("playerName"),
                        IconUrl = reader.GetString("iconUrl"),
                        TwitterInfo = reader.GetJsonObject<TwitterInfoClass>("twitter"),
                        ContactsInfo = reader.GetJsonObject<ContactsInfo>("contacts"),
                        TinderInfo = reader.GetJsonObject<TinderInfoClass>("tinder"),
                        ThemeInfo = reader.GetJsonObject<ThemeInfo>("theme"),
                        SMSInfo = reader.GetJsonObject<SMSInfo>("messages"),
                        NotificationInfo = reader.GetJsonObject<NotificationInfo>("notification"),
                        Notes = reader.GetJsonArray<NoteClass>("notes"),
                        LockScreenInfo = reader.GetJsonObject<LockScreenInfo>("lockscreen"),
                        InstagramInfo = reader.GetJsonObject<InstagramInfoClass>("instagram"),
                        GalleryUrls = reader.GetJsonArray<string>("galleryUrls"),
                        EmailInfo = reader.GetJsonObject<EmailInfoClass>("mail"),
                        DarkChatNames = reader.GetJsonArray<int>("darkchats"),
                        InstalledApps = reader.GetJsonArray<string>("installedApps"),
                        Transactions = reader.GetJsonArray<Transaction>("transactions")
                    };
                    numList.Add(num);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException("Error in Database -> GetPlayerAccounts():");
                Logger.LogError(ex.ToString());
            }
            return numList;
        }

        public PlayerAccount GetPlayerAccount(ulong steamID)
        {
            PlayerAccount account = null;

            try
            {
                MySqlConnection connection = this.createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@SID", steamID);
                command.CommandText = "select * from " + config.DatabaseData.Table_playerdata + " where steamID=@SID LIMIT 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    account = new PlayerAccount()
                    {
                        PlayerSteamID = reader.GetUInt64("steamID"),
                        PhoneNumber = reader.GetString("phoneNumber"),
                        PlayerLastName = reader.GetString("playerName"),
                        IconUrl = reader.GetString("iconUrl"),
                        TwitterInfo = reader.GetJsonObject<TwitterInfoClass>("twitter"),
                        ContactsInfo = reader.GetJsonObject<ContactsInfo>("contacts"),
                        TinderInfo = reader.GetJsonObject<TinderInfoClass>("tinder"),
                        ThemeInfo = reader.GetJsonObject<ThemeInfo>("theme"),
                        SMSInfo = reader.GetJsonObject<SMSInfo>("messages"),
                        NotificationInfo = reader.GetJsonObject<NotificationInfo>("notification"),
                        Notes = reader.GetJsonArray<NoteClass>("notes"),
                        LockScreenInfo = reader.GetJsonObject<LockScreenInfo>("lockscreen"),
                        InstagramInfo = reader.GetJsonObject<InstagramInfoClass>("instagram"),
                        GalleryUrls = reader.GetJsonArray<string>("galleryUrls"),
                        EmailInfo = reader.GetJsonObject<EmailInfoClass>("mail"),
                        DarkChatNames = reader.GetJsonArray<int>("darkchats"),
                        InstalledApps = reader.GetJsonArray<string>("installedApps"),
                        Transactions = reader.GetJsonArray<Transaction>("transactions")
                    };
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException("Error in Database -> GetPlayerAccount(STEAMID):");
                Logger.LogError(ex.ToString());
            }

            return account;
        }

        public PlayerAccount GetPlayerAccount(string number)
        {
            PlayerAccount account = null;
            try
            {
                MySqlConnection connection = this.createConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@NUM", number);
                command.CommandText = "select * from " + config.DatabaseData.Table_playerdata + " where phoneNumber=@NUM LIMIT 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    account = new PlayerAccount()
                    {
                        PlayerSteamID = reader.GetUInt64("steamID"),
                        PhoneNumber = reader.GetString("phoneNumber"),
                        PlayerLastName = reader.GetString("playerName"),
                        IconUrl = reader.GetString("iconUrl"),
                        TwitterInfo = reader.GetJsonObject<TwitterInfoClass>("twitter"),
                        ContactsInfo = reader.GetJsonObject<ContactsInfo>("contacts"),
                        TinderInfo = reader.GetJsonObject<TinderInfoClass>("tinder"),
                        ThemeInfo = reader.GetJsonObject<ThemeInfo>("theme"),
                        SMSInfo = reader.GetJsonObject<SMSInfo>("messages"),
                        NotificationInfo = reader.GetJsonObject<NotificationInfo>("notification"),
                        Notes = reader.GetJsonArray<NoteClass>("notes"),
                        LockScreenInfo = reader.GetJsonObject<LockScreenInfo>("lockscreen"),
                        InstagramInfo = reader.GetJsonObject<InstagramInfoClass>("instagram"),
                        GalleryUrls = reader.GetJsonArray<string>("galleryUrls"),
                        EmailInfo = reader.GetJsonObject<EmailInfoClass>("mail"),
                        DarkChatNames = reader.GetJsonArray<int>("darkchats"),
                        InstalledApps = reader.GetJsonArray<string>("installedApps"),
                        Transactions = reader.GetJsonArray<Transaction>("transactions")
                    };
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException("Error in Database -> GetPlayerAccounts(NUMBER):");
                Logger.LogError(ex.ToString());
            }
            return account;
        }

        public void AddPlayerAccount(PlayerAccount newAccount)
        {
            try
            {
                MySqlConnection connection = this.createConnection();
                MySqlCommand command = connection.CreateCommand();

                command.Parameters.AddWithValue("@SteamID", newAccount.PlayerSteamID);
                command.Parameters.AddWithValue("@PhoneNumber", newAccount.PhoneNumber);
                command.Parameters.AddWithValue("@Name", newAccount.PlayerLastName);
                command.Parameters.AddWithValue("@Icon", newAccount.IconUrl);
                command.Parameters.AddWithValue("@Contacts", JObject.FromObject(newAccount.ContactsInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@DChat", JArray.FromObject(newAccount.DarkChatNames).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Email", JObject.FromObject(newAccount.EmailInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Gallery", JArray.FromObject(newAccount.GalleryUrls).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Instagram", JObject.FromObject(newAccount.InstagramInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Apps", JArray.FromObject(newAccount.InstalledApps).ToString(Formatting.None));
                command.Parameters.AddWithValue("@LScreen", JObject.FromObject(newAccount.LockScreenInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Notes", JArray.FromObject(newAccount.Notes).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Notifies", JObject.FromObject(newAccount.NotificationInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@SMS", JObject.FromObject(newAccount.SMSInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Theme", JObject.FromObject(newAccount.ThemeInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Tinder", JObject.FromObject(newAccount.TinderInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Twitter", JObject.FromObject(newAccount.TwitterInfo).ToString(Formatting.None));
                command.Parameters.AddWithValue("@Trans", JArray.FromObject(newAccount.Transactions).ToString(Formatting.None));

                command.CommandText = "insert into `" + config.DatabaseData.Table_playerdata + "` SET " +
                    "steamID=@SteamID," +
                    "playerName=@Name," +
                    "phoneNumber=@PhoneNumber," +
                    "iconUrl=@Icon," +
                    "installedApps=@Apps," +
                    "galleryUrls=@Gallery," +
                    "theme=@Theme," +
                    "notification=@Notifies," +
                    "lockscreen=@LScreen," +
                    "contacts=@Contacts," +
                    "messages=@SMS," +
                    "twitter=@Twitter," +
                    "mail=@Email," +
                    "instagram=@Instagram," +
                    "tinder=@Tinder," +
                    "notes=@Notes," +
                    "transactions=@Trans," +
                    "darkchats=@DChat;";
                connection.Open();
                IAsyncResult asyncResult = command.BeginExecuteNonQuery();
                command.EndExecuteNonQuery(asyncResult);
                connection.Close();

                PhoneEventsManager.FPlayerAccountCreated((CSteamID)newAccount.PlayerSteamID, newAccount);
            }
            catch (Exception ex)
            {
                Logger.LogException("Error in Database -> AddPlayerAccount():");
                Logger.LogError(ex.ToString());
            }
        }

        public void RemovePlayerAccount(ulong steamID)
        {
            try
            {
                MySqlConnection connection = this.createConnection();
                MySqlCommand command = connection.CreateCommand();
                connection.Open();
                command.Parameters.AddWithValue("@SID", steamID);
                command.CommandText = "SELECT * FROM " + config.DatabaseData.Table_playerdata + " WHERE steamID=@SID";
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    command.CommandText = "delete from `" + config.DatabaseData.Table_playerdata + "` where steamID=@SID limit 1;";
                    IAsyncResult asyncResult = command.BeginExecuteNonQuery();
                    command.EndExecuteNonQuery(asyncResult);
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                Logger.LogException("Error in Database -> RemovePlayerAccount():");
                Logger.LogError(ex.ToString());
            }
        }

        public void UpdatePlayerAccount(ulong steamID, PlayerAccount accountData)
        {
            try
            {
                MySqlConnection MySQLConnection = createConnection();
                MySqlCommand command = MySQLConnection.CreateCommand();
                command.Parameters.AddWithValue("@SID", steamID);
                MySQLConnection.Open();

                command.CommandText = "SELECT * FROM " + config.DatabaseData.Table_playerdata + " WHERE steamID=@SID";
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    command.Parameters.AddWithValue("@PhoneNumber", accountData.PhoneNumber);
                    command.Parameters.AddWithValue("@Name", accountData.PlayerLastName);
                    command.Parameters.AddWithValue("@Icon", accountData.IconUrl);
                    command.Parameters.AddWithValue("@Contacts", JObject.FromObject(accountData.ContactsInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@DChat", JArray.FromObject(accountData.DarkChatNames).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Email", JObject.FromObject(accountData.EmailInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Gallery", JArray.FromObject(accountData.GalleryUrls).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Instagram", JObject.FromObject(accountData.InstagramInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Apps", JArray.FromObject(accountData.InstalledApps).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@LScreen", JObject.FromObject(accountData.LockScreenInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Notes", JArray.FromObject(accountData.Notes).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Notifies", JObject.FromObject(accountData.NotificationInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@SMS", JObject.FromObject(accountData.SMSInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Theme", JObject.FromObject(accountData.ThemeInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Tinder", JObject.FromObject(accountData.TinderInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Twitter", JObject.FromObject(accountData.TwitterInfo).ToString(Formatting.None));
                    command.Parameters.AddWithValue("@Trans", JArray.FromObject(accountData.Transactions).ToString(Formatting.None));

                    command.CommandText = "UPDATE `" + config.DatabaseData.Table_playerdata + "` SET " +
                        "playerName=@Name," +
                        "phoneNumber=@PhoneNumber," +
                        "iconUrl=@Icon," +
                        "installedApps=@Apps," +
                        "galleryUrls=@Gallery," +
                        "theme=@Theme," +
                        "notification=@Notifies," +
                        "lockscreen=@LScreen," +
                        "contacts=@Contacts," +
                        "messages=@SMS," +
                        "twitter=@Twitter," +
                        "mail=@Email," +
                        "instagram=@Instagram," +
                        "tinder=@Tinder," +
                        "notes=@Notes," +
                        "transactions=@Trans," +
                        "darkchats=@DChat WHERE steamID=@SID LIMIT 1;";
                    command.ExecuteNonQuery();
                }

                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException("Error in Database -> UpdatePlayerAccount():");
                Logger.LogError(ex.ToString());
            }
        }
        */
        #endregion
    }
}
