using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Managers;

namespace Tavstal.TLibrary.Extensions
{
    public static class MySqlExtensions
    {
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

        /// <summary>
        /// Safely creates the mysql connection, made because of very long console error that will scare the regular user and they will spam you with "it does not work" instead of checking the config.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task<bool> OpenSafe(this MySqlConnection connection)
        {
            if (connection.State == System.Data.ConnectionState.Open)
                return true;

            if (connection.State == System.Data.ConnectionState.Connecting)
                return true;

            try
            {
                await connection.OpenAsync();
                return true;
            }
            catch (MySqlException myex)
            {
                if (IsAuthenticationError(myex))
                {
                    LoggerHelper.LogWarning("# Failed to connect to the database due to authentication error. Please check the plugin's config file.");
                    LoggerHelper.LogError($"{myex}");
                    if (connection.State != System.Data.ConnectionState.Closed)
                        await connection.CloseAsync();
                    return false;
                }

                LoggerHelper.LogException($"Mysql error in TLibrary:");
                LoggerHelper.LogError(myex);
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
                return false;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
                return false;
            }
        }

        private static bool IsAuthenticationError(MySqlException ex)
        {
            return ex.ToString().StartsWith("MySql.Data.MySqlClient.MySqlException (0x80004005)") || ex.ToString().Contains("Access denied for user");
        }
    
    
        public static string GetString(this DbDataReader reader, string key)
        {
            return reader.GetString(reader.GetOrdinal(key));
        }

        public static DateTime GetDateTime(this DbDataReader reader, string column)
        {
            return reader.GetDateTime(reader.GetOrdinal(column));
        }

        public static decimal GetDecimal(this DbDataReader reader, string column)
        {
            return reader.GetDecimal(reader.GetOrdinal(column));
        }

        public static double GetDouble(this DbDataReader reader, string column)
        {
            return reader.GetDouble(reader.GetOrdinal(column));
        }

        public static Type GetFieldType(this DbDataReader reader, string column)
        {
            return reader.GetFieldType(reader.GetOrdinal(column));
        }

        public static float GetFloat(this DbDataReader reader, string column)
        {
            return reader.GetFloat(reader.GetOrdinal(column));
        }

        public static Guid GetGuid(this DbDataReader reader, string column)
        {
            return reader.GetGuid(reader.GetOrdinal(column));
        }

        public static short GetInt16(this DbDataReader reader, string column)
        {
            return reader.GetInt16(reader.GetOrdinal(column));
        }

        public static int GetInt32(this DbDataReader reader, string column)
        {
            return reader.GetInt32(reader.GetOrdinal(column));
        }

        public static long GetInt64(this DbDataReader reader, string column)
        {
            return reader.GetInt64(reader.GetOrdinal(column));
        }

        public static ushort GetUInt16(this DbDataReader reader, string column)
        {
            return (ushort)reader.GetInt16(reader.GetOrdinal(column));
        }

        public static uint GetUInt32(this DbDataReader reader, string column)
        {
            return (uint)reader.GetInt32(reader.GetOrdinal(column));
        }

        public static ulong GetUInt64(this DbDataReader reader, string column)
        {
            return (ulong)reader.GetInt64(reader.GetOrdinal(column));
        }
    }
}
