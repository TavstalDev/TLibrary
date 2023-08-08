using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Managers;

namespace Tavstal.TLibrary.Extensions
{
    public static class MySqlExtensions
    {
        /// <summary>
        /// Used to prevent mysql auth error spam
        /// </summary>
        public static bool IsConnectionAuthFailed { get; private set; }

        /// <summary>
        /// Retrieves a JSON object of type T from the specified column in the <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize the JSON to.</typeparam>
        /// <param name="reader">The MySqlDataReader containing the data.</param>
        /// <param name="column">The name of the column containing the JSON data.</param>
        /// <returns>The deserialized object of type T.</returns>
        public static T GetJsonObject<T>(this MySqlDataReader reader, string column)
        {
            return JsonConvert.DeserializeObject<T>(reader.GetString(column));
        }

        /// <summary>
        /// Retrieves a JSON array of objects of type T from the specified column in the <paramref name="reader"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects in the JSON array.</typeparam>
        /// <param name="reader">The MySqlDataReader containing the data.</param>
        /// <param name="column">The name of the column containing the JSON array data.</param>
        /// <returns>A List of objects of type T deserialized from the JSON array.</returns>
        public static List<T> GetJsonArray<T>(this MySqlDataReader reader, string column)
        {
            return JsonConvert.DeserializeObject<List<T>>(reader.GetString(column));
        }

        /// <summary>
        /// Safely retrieves a floating-point value from the specified column in the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The MySqlDataReader containing the data.</param>
        /// <param name="column">The name of the column containing the floating-point value.</param>
        /// <returns>The floating-point value from the specified column or a default value if invalid or null.</returns>
        public static float GetFloatSafe(this MySqlDataReader reader, string column)
        {
            return Convert.ToSingle(reader.GetString(column).Replace(".", ","));
        }

        /// <summary>
        /// Safely retrieves a decimal value from the specified column in the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The MySqlDataReader containing the data.</param>
        /// <param name="column">The name of the column containing the decimal value.</param>
        /// <returns>The decimal value from the specified column or a default value if invalid or null.</returns>
        public static decimal GetDecimalSafe(this MySqlDataReader reader, string column)
        {
            return Convert.ToDecimal(reader.GetString(column).Replace(".", ","));
        }

        /// <summary>
        /// Safely retrieves a double value from the specified column in the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The MySqlDataReader containing the data.</param>
        /// <param name="column">The name of the column containing the double value.</param>
        /// <returns>The double value from the specified column or a default value if invalid or null.</returns>
        public static double GetDoubleSafe(this MySqlDataReader reader, string column)
        {
            return Convert.ToDouble(reader.GetString(column).Replace(".", ","));
        }

        public static bool OpenSafe(this MySqlConnection connection)
        {
            if (IsConnectionAuthFailed)
                return false;

            if (connection.State == System.Data.ConnectionState.Open)
                return true;

            if (connection.State == System.Data.ConnectionState.Connecting)
                return true;

            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException myex)
            {
                if (IsAuthenticationError(myex))
                {
                    IsConnectionAuthFailed = true;
                    LoggerHelper.LogWarning("Failed to connect to the database. Please check your config file.");
                    LoggerHelper.LogError($"{myex.Number} : {myex}");
                    return false;
                }

                LoggerHelper.LogException("Mysql error in TLibrary:");
                LoggerHelper.LogError(myex);
                return false;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return false;
            }
        }

        internal static bool IsAuthenticationError(MySqlException ex)
        {
            // List of MySQL error codes related to authentication
            int[] authErrorCodes = { 1045, 1049, 1698, /* Add more codes if needed */ };

            return authErrorCodes.Contains(ex.Number);
        }
    }
}
