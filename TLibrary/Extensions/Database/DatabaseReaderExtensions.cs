using System;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using MySqlConnector;
using Tavstal.TLibrary.Helpers.Database;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TLibrary.Extensions.Database
{
    /// <summary>
    /// Provides extension methods for MySQL and database operations.
    /// </summary>
    public static class DatabaseReaderExtensions
    {
        /// <summary>
        /// Converts the current row of a <see cref="MySqlDataReader"/> to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to map the row to.</typeparam>
        /// <param name="reader">The MySQL data reader containing the row data.</param>
        /// <returns>An instance of <typeparamref name="T"/> populated with the row values, or <see langword="null"/> if the reader has no rows or an error occurs.</returns>
        public static T? ConvertToObject<T>(this MySqlDataReader reader) where T : class
        {
            try
            {
                if (!reader.HasRows)
                    return null;

                T obj = Activator.CreateInstance<T>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string propName = prop.Name;
                    var memberAtt = prop.GetCustomAttribute<SqlMemberAttribute>();
                    if (memberAtt != null)
                    {
                        if (!string.IsNullOrEmpty(memberAtt.ColumnName))
                            propName = memberAtt.ColumnName!;
                    }

                    int ordinal = reader.GetOrdinal(propName);
                    var value = reader.GetValue(ordinal);

                    if (value == DBNull.Value)
                    {
                        prop.SetValue(obj, null);
                        continue;
                    }

                    if (prop.PropertyType.IsEnum)
                        prop.SetValue(obj, Enum.ToObject(prop.PropertyType, value));
                    else
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(obj,
                                prop.PropertyType.Name == value.GetType().Name
                                    ? SqlTypeHelper.ConvertSqlToIllegalChars((string)value)
                                    : SqlTypeHelper.ConvertSqlToIllegalChars((string)Convert.ChangeType(value, prop.PropertyType)));
                        }
                        else
                        {
                            prop.SetValue(obj,
                                prop.PropertyType.Name == value.GetType().Name
                                    ? value
                                    : Convert.ChangeType(value, prop.PropertyType));
                        }
                    }
                }

                return obj;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return null;
            }
        }
        
        /// <summary>
        /// Converts the current row of a <see cref="DbDataReader"/> to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to map the row to.</typeparam>
        /// <param name="reader">The database data reader containing the row data.</param>
        /// <returns>An instance of <typeparamref name="T"/> populated with the row values, or <see langword="null"/> if the reader has no rows or an error occurs.</returns>
        public static T? ConvertToObject<T>(this DbDataReader reader) where T : class
        {
            try
            {
                if (!reader.HasRows)
                    return null;

                T obj = Activator.CreateInstance<T>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string propName = prop.Name;
                    var memberAtt = prop.GetCustomAttribute<SqlMemberAttribute>();
                    if (memberAtt != null)
                    {
                        if (!string.IsNullOrEmpty(memberAtt.ColumnName))
                            propName = memberAtt.ColumnName!;
                    }

                    int ordinal = reader.GetOrdinal(propName);
                    var value = reader.GetValue(ordinal);

                    if (prop.PropertyType.IsEnum)
                        prop.SetValue(obj, Enum.ToObject(prop.PropertyType, value));
                    else
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(obj,
                                prop.PropertyType.Name == value.GetType().Name
                                    ? SqlTypeHelper.ConvertSqlToIllegalChars((string)value)
                                    : SqlTypeHelper.ConvertSqlToIllegalChars((string)Convert.ChangeType(value, prop.PropertyType)));
                        }
                        else
                        {
                            prop.SetValue(obj,
                                prop.PropertyType.Name == value.GetType().Name
                                    ? value
                                    : Convert.ChangeType(value, prop.PropertyType));
                        }
                    }
                }

                return obj;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return null;
            }
        }

        /// <summary>
        /// Safely reads a float value from a column.
        /// </summary>
        /// <param name="reader">The MySQL data reader.</param>
        /// <param name="column">The column name.</param>
        /// <returns>The float value, or a default if invalid.</returns>
        public static float GetFloatSafe(this MySqlDataReader reader, string column) =>
            Convert.ToSingle(reader.GetString(column).Replace(".", ","));

        /// <summary>
        /// Safely reads a decimal value from a column.
        /// </summary>
        /// <param name="reader">The MySQL data reader.</param>
        /// <param name="column">The column name.</param>
        /// <returns>The decimal value, or a default if invalid.</returns>
        public static decimal GetDecimalSafe(this MySqlDataReader reader, string column) =>
            Convert.ToDecimal(reader.GetString(column).Replace(".", ","));

        /// <summary>
        /// Safely reads a double value from a column.
        /// </summary>
        /// <param name="reader">The MySQL data reader.</param>
        /// <param name="column">The column name.</param>
        /// <returns>The double value, or a default if invalid.</returns>
        public static double GetDoubleSafe(this MySqlDataReader reader, string column) =>
            Convert.ToDouble(reader.GetString(column).Replace(".", ","));

        /// <summary>
        /// Safely opens a MySQL connection.
        /// Shows a simple error message instead of a long one that might confuse users.
        /// </summary>
        /// <param name="connection">The MySQL connection.</param>
        /// <returns>True if connected, false if it failed.</returns>
        public static async Task<EDatabaseState> OpenSafeAsync(this MySqlConnection connection)
        {
            if (connection.State == System.Data.ConnectionState.Open)
                return EDatabaseState.SUCCESS;

            if (connection.State == System.Data.ConnectionState.Connecting)
                return EDatabaseState.SUCCESS;

            try
            {
                await connection.OpenAsync();
                return EDatabaseState.SUCCESS;
            }
            catch (MySqlException myex)
            {
                if (IsAuthenticationError(myex))
                {
                    LoggerHelper.LogWarning(
                        "# Failed to connect to the database due to authentication error. Please check the plugin's config file.");
                    LoggerHelper.LogError($"{myex}");
                    if (connection.State != System.Data.ConnectionState.Closed)
                        await connection.CloseAsync();
                    return EDatabaseState.AUTHENTICATION_FAILED;
                }

                LoggerHelper.LogError($"Mysql error in TLibrary:");
                LoggerHelper.LogError(myex);
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
                return EDatabaseState.ERROR;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != System.Data.ConnectionState.Closed)
                    await connection.CloseAsync();
                return EDatabaseState.ERROR;
            }
        }

        /// <summary>
        /// Checks if a MySQL error is an authentication problem (wrong username or password).
        /// </summary>
        /// <param name="ex">The MySQL error to check.</param>
        /// <returns>True if it is an authentication error, false if not.</returns>
        private static bool IsAuthenticationError(MySqlException ex) =>
            ex.ToString().StartsWith("MySql.Data.MySqlClient.MySqlException (0x80004005)") ||
            ex.ToString().Contains("Access denied for user");

        /// <summary>
        /// Reads a string value from a column by its name.
        /// </summary>
        /// <param name="reader">The database reader.</param>
        /// <param name="key">The column name.</param>
        /// <returns>The string value.</returns>
        public static string GetString(this DbDataReader reader, string key) =>
            reader.GetString(reader.GetOrdinal(key));

        /// <summary>
        /// Retrieves the <see cref="DateTime"/> value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose <see cref="DateTime"/> value is to be retrieved.</param>
        /// <returns>The <see cref="DateTime"/> value of the specified column.</returns>
        public static DateTime GetDateTime(this DbDataReader reader, string column) => 
            reader.GetDateTime(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the decimal value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose decimal value is to be retrieved.</param>
        /// <returns>The decimal value of the specified column.</returns>
        public static decimal GetDecimal(this DbDataReader reader, string column) => 
            reader.GetDecimal(reader.GetOrdinal(column));
        
        /// <summary>
        /// Retrieves the double value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose double value is to be retrieved.</param>
        /// <returns>The double value of the specified column.</returns>
        public static double GetDouble(this DbDataReader reader, string column) => 
            reader.GetDouble(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the data type of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the column's data type.</param>
        /// <param name="column">The name of the column whose data type is to be retrieved.</param>
        /// <returns>The <see cref="Type"/> of the specified column.</returns>
        public static Type? GetFieldType(this DbDataReader reader, string column) =>
            reader.GetFieldType(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the float value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose float value is to be retrieved.</param>
        /// <returns>The float value of the specified column.</returns>
        public static float GetFloat(this DbDataReader reader, string column) =>
            reader.GetFloat(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the GUID value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the GUID.</param>
        /// <param name="column">The name of the column whose GUID value is to be retrieved.</param>
        /// <returns>The <see cref="Guid"/> value of the specified column.</returns>
        public static Guid GetGuid(this DbDataReader reader, string column) =>
            reader.GetGuid(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the 16-bit integer value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose 16-bit integer value is to be retrieved.</param>
        /// <returns>The 16-bit integer value of the specified column.</returns>
        public static short GetInt16(this DbDataReader reader, string column) =>
            reader.GetInt16(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the 32-bit integer value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose 32-bit integer value is to be retrieved.</param>
        /// <returns>The 32-bit integer value of the specified column.</returns>
        public static int GetInt32(this DbDataReader reader, string column) =>
            reader.GetInt32(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the 64-bit integer value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose 64-bit integer value is to be retrieved.</param>
        /// <returns>The 64-bit integer value of the specified column.</returns>
        public static long GetInt64(this DbDataReader reader, string column) =>
            reader.GetInt64(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the unsigned 16-bit integer value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose unsigned 16-bit integer value is to be retrieved.</param>
        /// <returns>The unsigned 16-bit integer value of the specified column.</returns>
        public static ushort GetUInt16(this DbDataReader reader, string column) =>
            (ushort)reader.GetInt16(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the unsigned 32-bit integer value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose unsigned 32-bit integer value is to be retrieved.</param>
        /// <returns>The unsigned 32-bit integer value of the specified column.</returns>
        public static uint GetUInt32(this DbDataReader reader, string column) =>
            (uint)reader.GetInt32(reader.GetOrdinal(column));

        /// <summary>
        /// Retrieves the unsigned 64-bit integer value of the specified column from the current row in the <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance from which to retrieve the value.</param>
        /// <param name="column">The name of the column whose unsigned 64-bit integer value is to be retrieved.</param>
        /// <returns>The unsigned 64-bit integer value of the specified column.</returns>
        public static ulong GetUInt64(this DbDataReader reader, string column) =>
            (ulong)reader.GetInt64(reader.GetOrdinal(column));
    }
}
