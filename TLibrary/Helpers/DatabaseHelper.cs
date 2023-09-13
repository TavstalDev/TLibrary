using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Data;
using Tavstal.TLibrary.Compatibility.Database;
using Tavstal.TLibrary.Extensions;
using SDG.Unturned;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using Newtonsoft.Json.Linq;
using MySqlX.XDevAPI.Common;
using System.Web.Compilation;
using System.Web.UI.WebControls;

namespace Tavstal.TLibrary.Helpers
{
    public static class DatabaseHelper
    {
        /// <summary>
        /// Converts a C# data type to its corresponding SQL data type.
        /// </summary>
        /// <param name="type">The C# data type to be converted.</param>
        /// <param name="length">Optional. The length or size of the data type if applicable.</param>
        /// <returns>The corresponding SQL data type as a string.</returns>
        public static string ConvertToSqlDataType(Type type, int? length = null)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return "BOOL";
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return "TINYINT";
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    return "SMALLINT";
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return length.HasValue ? $"INT({length.Value})" : "INT";
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return "BIGINT";
                case TypeCode.Single:
                    return "REAL";
                case TypeCode.Double:
                    return "FLOAT";
                case TypeCode.Decimal:
                    return "DECIMAL";
                case TypeCode.Char:
                    return "CHAR";
                case TypeCode.String:
                    return length.HasValue ? $"VARCHAR({length.Value})" : "TEXT";
                case TypeCode.DateTime:
                    return "DATETIME";
                default:
                    if (type == typeof(Guid))
                    {
                        return "LONGTEXT";
                    }
                    else if (type == typeof(byte[]))
                    {
                        return "VARBINARY(MAX)";
                    }
                    else
                    {
                        // Custom or unsupported data type
                        throw new NotSupportedException($"Data type '{type.FullName}' is not supported.");
                    }
            }
        }

        /// <summary>
        /// Converts an SQL data type to its corresponding C# data type.
        /// </summary>
        /// <param name="sqlDataType">The SQL data type to be converted.</param>
        /// <returns>The corresponding C# data type as a Type object.</returns>
        public static Type ConvertSqlToCSharpDataType(string sqlDataType)
        {
            if (sqlDataType == "TINYINT(1)")
                sqlDataType = "BOOL";

            if (sqlDataType.Contains('('))
                sqlDataType = sqlDataType.Split('(')[0];

            switch (sqlDataType.Replace(" ", "").ToUpper())
            {
                case "BIT":
                case "BOOL":
                case "BOOLEAN":
                    return typeof(bool);
                case "TINYINT":
                    return typeof(byte);
                case "SMALLINT":
                    return typeof(short);
                case "INT":
                    return typeof(int);
                case "BIGINT":
                    return typeof(long);
                case "REAL":
                    return typeof(float);
                case "FLOAT":
                    return typeof(double);
                case "DECIMAL":
                    return typeof(decimal);
                case "CHAR":
                case "VARCHAR":
                    return typeof(string);
                case "DATETIME":
                    return typeof(DateTime);
                case "LONGTEXT":
                    {
                        return typeof(Guid);
                    }
                case "TEXT":
                    {
                        return typeof(string);
                    }
                case "VARBINARY":
                    return typeof(byte[]);
                default:
                    throw new NotSupportedException($"SQL data type '{sqlDataType}' is not supported.");
            }
        }

        private static string ConvertIllegalCharsToSql(string value)
        {
            return value.Replace("'", "U+0027");
        }

        private static string ConvertSqlToIllegalChars(string value)
        {
            return value.Replace("U+0027", "'");
        }

        private static TEnum ParseEnum<TEnum>(Type enumType, int value)
        {
            Array enumValues = Enum.GetValues(enumType);
            TEnum localEnum = default;

            foreach (object enumValue in enumValues)
            {
                int enumIntValue = (int)enumValue;
                if (enumIntValue == value)
                {
                    localEnum = (TEnum)enumValue;
                    break;
                }
            }
            return localEnum;
        }

        /// <summary>
        /// Converts the current row in the MySqlDataReader to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to be converted.</typeparam>
        /// <param name="reader">The MySqlDataReader to read data from.</param>
        /// <returns>An object of the specified type representing the current row data, or default(T) if the conversion fails.</returns>
        public static T ConvertToObject<T>(this MySqlDataReader reader) where T : class
        {
            try
            {
                if (!reader.HasRows)
                    return default;

                T obj = Activator.CreateInstance<T>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string propName = prop.Name;
                    var memberAtt = prop.GetCustomAttribute<SqlMemberAttribute>();
                    if (memberAtt != null)
                    {
                        if (!memberAtt.ColumnName.IsNullOrEmpty())
                            propName = memberAtt.ColumnName;
                    }

                    int ordinal = reader.GetOrdinal(propName);
                    var value = reader.GetValue(ordinal);

                    if (prop.PropertyType.IsEnum)
                        prop.SetValue(obj, Enum.ToObject(prop.PropertyType, value));
                    else
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            if (prop.PropertyType.Name == value.GetType().Name)
                                prop.SetValue(obj, ConvertSqlToIllegalChars((string)value));
                            else
                                prop.SetValue(obj, ConvertSqlToIllegalChars((string)Convert.ChangeType(value, prop.PropertyType)));
                        }
                        else
                        {
                            if (prop.PropertyType.Name == value.GetType().Name)
                                prop.SetValue(obj, value);
                            else
                                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                        }
                    }
                }

                /*foreach (var prop in typeof(T).GetFields())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string propName = prop.Name;
                    var memberAtt = prop.GetCustomAttribute<SqlMemberAttribute>();
                    if (memberAtt != null)
                    {
                        if (!memberAtt.ColumnName.IsNullOrEmpty())
                            propName = memberAtt.ColumnName;
                    }

                    int ordinal = reader.GetOrdinal(propName);
                    var value = reader.GetValue(ordinal);

                    if (prop.FieldType.Name == value.GetType().Name)
                        prop.SetValue(obj, value);
                    else
                        prop.SetValue(obj, Convert.ChangeType(value, prop.FieldType));
                }*/

                return obj;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }

        /// <summary>
        /// Checks if a table with the specified name exists in the database.
        /// </summary>
        /// <typeparam name="T">The type associated with the table.</typeparam>
        /// <param name="connection">The MySQL database connection.</param>
        /// <param name="tableName">The name of the table to check.</param>
        /// <returns><c>true</c> if the table exists, otherwise <c>false</c>.</returns>
        public static bool DoesTableExist<T>(this MySqlConnection connection, string tableName) where T : class
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.CommandText = $"SHOW TABLES LIKE @TableName;";

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        connection.Close();
                        return true;
                    }
                }
                connection.Close();
                return false;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Checks if a table associated with the specified type exists in the database.
        /// </summary>
        /// <typeparam name="T">The type associated with the table.</typeparam>
        /// <param name="connection">The MySQL database connection.</param>
        /// <returns><c>true</c> if the table exists, otherwise <c>false</c>.</returns>
        public static bool DoesTableExist<T>(this MySqlConnection connection) where T : class
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");
                return DoesTableExist<T>(connection, attribute.Name);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Creates a new database table for the specified type T in the MySQL database.
        /// </summary>
        /// <typeparam name="T">The type of object for which the table is created.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table to be created.</param>
        /// <returns>True if the table creation is successful, otherwise false.</returns>
        public static bool CreateTable<T>(this MySqlConnection connection, string tableName) where T : class
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);

                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.CommandText = $"SHOW TABLES LIKE @TableName;";
                    object result = command.ExecuteScalar();
                    if (result != null)
                        return false;
                }

                string schemaParams = string.Empty;
                string keyParams = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string propName = prop.Name;
                    string typeName = ConvertToSqlDataType(prop.PropertyType);
                    string nullableString = string.Empty;
                    string unsignedString = string.Empty;
                    string autoincrementString = string.Empty;
                    var sqlMember = prop.GetCustomAttribute<SqlMemberAttribute>();

                    if (sqlMember != null)
                    {
                        if (!sqlMember.ColumnName.IsNullOrEmpty())
                            propName = sqlMember.ColumnName;

                        if (!sqlMember.ColumnType.IsNullOrEmpty())
                            typeName = sqlMember.ColumnType;

                        if (sqlMember.ShouldAutoIncrement)
                            autoincrementString = $" AUTO_INCREMENT";

                        if (!sqlMember.IsNullable)
                            nullableString = $" NOT NULL";

                        if (sqlMember.IsUnsigned)
                            nullableString = $" UNSIGNED";

                        if (sqlMember.IsPrimaryKey)
                            keyParams += $"PRIMARY KEY({propName}),";
                        else if (sqlMember.IsUnique)
                            keyParams += $"UNIQUE ({propName}),";

                        if (sqlMember.IsForeignKey)
                            keyParams += $"FOREIGN KEY ({propName}) REFERENCES {sqlMember.ForeignTable}({sqlMember.ForeignColumn}),";
                    }
                    schemaParams += $"{propName} {typeName}{unsignedString}{nullableString}{autoincrementString},";
                }

                /*foreach (var prop in schemaType.GetFields())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string propName = prop.Name;
                    string typeName = ConvertToSqlDataType(prop.FieldType);
                    string nullableString = string.Empty;
                    string unsignedString = string.Empty;
                    string autoincrementString = string.Empty;
                    var sqlMember = prop.GetCustomAttribute<SqlMemberAttribute>();

                    if (sqlMember != null)
                    {
                        if (!sqlMember.ColumnName.IsNullOrEmpty())
                            propName = sqlMember.ColumnName;

                        if (!sqlMember.ColumnType.IsNullOrEmpty())
                            typeName = sqlMember.ColumnType;

                        if (sqlMember.ShouldAutoIncrement)
                            autoincrementString = $" AUTO_INCREMENT";

                        if (!sqlMember.IsNullable)
                            nullableString = $" NOT NULL";

                        if (sqlMember.IsUnsigned)
                            nullableString = $" UNSIGNED";

                        if (sqlMember.IsPrimaryKey)
                            keyParams += $"PRIMARY KEY({propName}),";
                        else if (sqlMember.IsUnique)
                            keyParams += $"UNIQUE ({propName}),";

                        if (sqlMember.IsForeignKey)
                            keyParams += $"FOREIGN KEY ({propName}) REFERENCES {sqlMember.ForeignTable}({sqlMember.ForeignColumn}),";
                    }
                    schemaParams += $"{propName} {typeName}{unsignedString}{nullableString}{autoincrementString},";
                }*/

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE TABLE {tableName} ({schemaParams}{keyParams})";
                    command.CommandText = command.CommandText.Remove(command.CommandText.LastIndexOf(','), 1);
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Creates a new database table for the specified type T in the MySQL database.
        /// </summary>
        /// <typeparam name="T">The type of object for which the table is created.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <returns>True if the table creation is successful, otherwise false.</returns>
        public static bool CreateTable<T>(this MySqlConnection connection) where T : class
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return CreateTable<T>(connection, attribute.Name);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Checks the structure of the database table for the specified type T in the MySQL database.
        /// If the table has missing columns, it adds or updates them.
        /// If the table has extra columns, it removes them.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table to check and update.</param>
        /// <returns>True if the table structure was successfully checked and updated, otherwise false.</returns>
        public static bool CheckTable<T>(this MySqlConnection connection, string tableName) where T : class
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);

                List<SqlColumn> classColumns = new List<SqlColumn>();
                #region Read T Fields and Properties
                foreach (var prop in schemaType.GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var sqlMember = prop.GetCustomAttribute<SqlMemberAttribute>();
                    SqlColumn localColumn = new SqlColumn();
                    localColumn.ColumnName = prop.Name;
                    localColumn.ColumnType = ConvertToSqlDataType(prop.PropertyType);

                    if (sqlMember != null)
                    {
                        localColumn = sqlMember.ToColumn();

                        if (localColumn.ColumnName.IsNullOrEmpty())
                            localColumn.ColumnName = prop.Name;

                        if (localColumn.ColumnType.IsNullOrEmpty())
                            localColumn.ColumnType = ConvertToSqlDataType(prop.PropertyType);
                    }
                    classColumns.Add(localColumn);
                }

                /*foreach (var prop in schemaType.GetFields())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var sqlMember = prop.GetCustomAttribute<SqlMemberAttribute>();
                    SqlColumn localColumn = new SqlColumn();
                    localColumn.ColumnName = prop.Name;
                    localColumn.ColumnType = ConvertToSqlDataType(prop.FieldType);

                    if (sqlMember != null)
                    {
                        localColumn = sqlMember.ToColumn();

                        if (localColumn.ColumnName.IsNullOrEmpty())
                            localColumn.ColumnName = prop.Name;

                        if (localColumn.ColumnType.IsNullOrEmpty())
                            localColumn.ColumnType = ConvertToSqlDataType(prop.FieldType);
                    }
                    classColumns.Add(localColumn);
                }*/
                #endregion

                connection.OpenSafe();
                List<SqlColumn> liveColumns = new List<SqlColumn>();
                List<SqlColumn> columnsToRemove = new List<SqlColumn>();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.CommandText = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE @TableName;";

                    object result = command.ExecuteScalar();
                    if (result == null)
                        return false;

                    command.CommandText = $"SHOW COLUMNS FROM `{tableName}`;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnKey = reader.GetString("Key");
                            string columnExtra = reader.GetString("Extra");
                            string type = reader.GetString("Type");

                            var localColumn = new SqlColumn(
                                columnName: reader.GetString("Field"),
                                columnType: type.Replace("unsigned", ""),
                                isNullable: reader.GetString("Null") == "YES",
                                isPrimaryKey: columnKey == "PRI",
                                isUnique: columnKey == "UNI",
                                isUnsigned: type.ContainsIgnoreCase("unsigned"),
                                shouldAutoIncrement: columnExtra.Contains("auto_increment"),
                                isForeignKey: false,
                                foreignTable: null,
                                foreignColumn: null);
                            liveColumns.Add(localColumn);

                            if (!classColumns.Any(x => x.ColumnName == localColumn.ColumnName))
                                columnsToRemove.Add(localColumn);
                        }
                    }
                }

                List<SqlColumn> columnsToAdd = new List<SqlColumn>();
                List<SqlColumn> columnsToUpdate = new List<SqlColumn>();
                #region Fill column lists
                foreach (var column in classColumns)
                {
                    var lcolumn = liveColumns.Find(x => x.ColumnName == column.ColumnName);
                    if (lcolumn == null)
                    {
                        columnsToAdd.Add(column);
                        continue;
                    }

                    //LoggerHelper.LogWarning($"TYPE {lcolumn.ColumnType.ToLower().Replace(" ", "")} - {column.ColumnType.ToLower().Replace(" ", "")}   --> {lcolumn.ColumnType.ToLower().Replace(" ", "") == column.ColumnType.ToLower().Replace(" ", "")}");

                    if (lcolumn.IsPrimaryKey != column.IsPrimaryKey || 
                        lcolumn.IsUnique != column.IsUnique ||
                        lcolumn.IsNullable != column.IsNullable ||
                        lcolumn.IsUnsigned != column.IsUnsigned ||
                        lcolumn.ShouldAutoIncrement != column.ShouldAutoIncrement ||
                        ConvertSqlToCSharpDataType(lcolumn.ColumnType) != ConvertSqlToCSharpDataType(column.ColumnType))
                    {
                        columnsToUpdate.Add(column);
                        continue;
                    }
                }
                #endregion

                if (columnsToAdd.Count == 0 && columnsToRemove.Count == 0 && columnsToUpdate.Count == 0)
                {
                    connection.Close();
                    return true;
                }

                #region Modify Table 
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"ALTER TABLE {tableName}";
                    // ADD
                    foreach (SqlColumn column in columnsToAdd)
                    {
                        string primaryKey = column.IsPrimaryKey ? " PRIMARY KEY" : "";
                        string nullable = column.IsNullable ? "" : " NOT NULL";
                        string unsigned = column.IsUnsigned ? " UNSIGNED" : "";
                        string uniqueKey = column.IsUnique ? " UNIQUE" : "";
                        string autoIncrement = column.ShouldAutoIncrement ? " AUTO_INCREMENT" : "";
                        command.CommandText += $" ADD {column.ColumnName} {column.ColumnType}{unsigned}{nullable}{uniqueKey}{primaryKey}{autoIncrement},";
                    }
                    // UPDATE
                    foreach (SqlColumn column in columnsToUpdate)
                    {
                        string primaryKey = column.IsPrimaryKey ? " PRIMARY KEY" : "";
                        string unsigned = column.IsUnsigned ? " UNSIGNED" : "";
                        string nullable = column.IsNullable ? "" : " NOT NULL";
                        string uniqueKey = column.IsUnique ? " UNIQUE" : "";
                        string autoIncrement = column.ShouldAutoIncrement ? " AUTO_INCREMENT" : "";
                        command.CommandText += $" MODIFY {column.ColumnName} {column.ColumnType}{unsigned}{nullable}{uniqueKey}{primaryKey}{autoIncrement},";
                    }
                    // DROP
                    foreach (SqlColumn column in columnsToRemove)
                    {
                        command.CommandText += $" DROP COLUMN {column.ColumnName},";
                    }

                    command.CommandText = command.CommandText.Remove(command.CommandText.LastIndexOf(','), 1);
                    command.ExecuteNonQuery();
                }
                #endregion

                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Checks the structure of the database table for the specified type T in the MySQL database.
        /// If the table has missing columns, it adds or updates them.
        /// If the table has extra columns, it removes them.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <returns>True if the table structure was successfully checked and updated, otherwise false.</returns>
        public static bool CheckTable<T>(this MySqlConnection connection) where T : class
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return CheckTable<T>(connection, attribute.Name);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Retrieves a list of objects of type T from the MySQL database table with the specified name.
        /// The retrieved rows are filtered using the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table from which to retrieve rows.</param>
        /// <param name="whereClause">The WHERE clause used to filter the rows (e.g., "Column1 = @param1 AND Column2 > @param2").</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <param name="limit">The maximum number of rows to retrieve (default is -1, which means no limit).</param>
        /// <returns>A list of objects of type T containing the retrieved rows from the table.</returns>
        public static List<T> GetTableRows<T>(this MySqlConnection connection, string tableName, string whereClause, List<MySqlParameter> parameters, int limit = -1) where T : class
        {
            if (connection == null)
                return null;

            try
            {
                List<T> localList = new List<T>();
                string limitClause = limit > 0 ? $" LIMIT {limit}" : "";
                string whereClauseExp = string.Empty;
                if (!whereClause.IsNullOrEmpty())
                {
                    if (!whereClause.StartsWith("WHERE "))
                        whereClause = "WHERE " + whereClause;
                }
                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    if (parameters != null)
                        foreach (var param in parameters)
                            command.Parameters.Add(param);
                    command.CommandText = $"SELECT * FROM {tableName} {whereClause}{limitClause}";

                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        localList.Add(ConvertToObject<T>(reader));
                    }
                }
                connection.Close();
                return localList;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of objects of type T from the MySQL database table with the specified name.
        /// The retrieved rows are filtered using the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="whereClause">The WHERE clause used to filter the rows (e.g., "Column1 = @param1 AND Column2 > @param2").</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <param name="limit">The maximum number of rows to retrieve (default is -1, which means no limit).</param>
        /// <returns>A list of objects of type T containing the retrieved rows from the table.</returns>
        public static List<T> GetTableRows<T>(this MySqlConnection connection, string whereClause, List<MySqlParameter> parameters, int limit = -1) where T : class
        {
            if (connection == null)
                return null;

            try
            {
                var attribute = typeof(T).GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return connection.GetTableRows<T>(attribute.Name, whereClause, parameters, limit);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return null;
            }
        }

        /// <summary>
        /// Retrieves a single object of type T from the MySQL database table with the specified name.
        /// The retrieved row is filtered using the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table from which to retrieve the row.</param>
        /// <param name="whereClause">The WHERE clause used to filter the row (e.g., "Column1 = @param1 AND Column2 > @param2").</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <returns>An object of type T representing the retrieved row from the table.</returns>
        public static T GetTableRow<T>(this MySqlConnection connection, string tableName, string whereClause, List<MySqlParameter> parameters) where T : class
        {
            if (connection == null)
                return null;

            try
            {
                var list = connection.GetTableRows<T>(tableName: tableName, whereClause: whereClause, parameters: parameters, limit: 1);
                if (list == null)
                    return null;

                if (list.Count <= 0)
                    return null;

                return list.ElementAt(0);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return null;
            }
        }

        /// <summary>
        /// Retrieves a single object of type T from the MySQL database table with the specified name.
        /// The retrieved row is filtered using the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="whereClause">The WHERE clause used to filter the row (e.g., "Column1 = @param1 AND Column2 > @param2").</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <returns>An object of type T representing the retrieved row from the table.</returns>
        public static T GetTableRow<T>(this MySqlConnection connection, string whereClause, List<MySqlParameter> parameters) where T : class
        {
            if (connection == null)
                return null;

            try
            { 
                var attribute = typeof(T).GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                var list = connection.GetTableRows<T>(tableName: attribute.Name, whereClause: whereClause, parameters: parameters, limit: 1);
                if (list == null)
                    return null;

                if (list.Count <= 0)
                    return null;

                return list.ElementAt(0);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return null;
            }
        }

        /// <summary>
        /// Adds a new row to the MySQL database table with the specified name.
        /// The row data is provided as an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table to which the row will be added.</param>
        /// <param name="value">The object representing the row data to be added.</param>
        /// <returns>True if the row was successfully added; otherwise, false.</returns>
        public static bool AddTableRow<T>(this MySqlConnection connection, string tableName, T value)
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                string paramString = string.Empty;
                string keyString = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var memberAttribute = prop.GetCustomAttribute<SqlMemberAttribute>();
                    string propName = prop.Name;

                    if (memberAttribute != null)
                    {
                        if (memberAttribute.ShouldAutoIncrement)
                            continue;

                        if (!memberAttribute.ColumnName.IsNullOrEmpty())
                            propName = memberAttribute.ColumnName;
                    }

                    keyString += $"{propName},";
                    if (prop.PropertyType == typeof(bool) || prop.PropertyType.IsEnum)
                        paramString += $"'{Convert.ToInt32(prop.GetValue(value))}',";
                    else if (prop.PropertyType == typeof(DateTime))
                        paramString += $"'{((DateTime)prop.GetValue(value)).ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
                    else if (prop.PropertyType == typeof(string))
                        paramString += $"'{ConvertIllegalCharsToSql((string)prop.GetValue(value))}',";
                    else
                        paramString += $"'{prop.GetValue(value)}',";
                }

                paramString = paramString.Remove(paramString.LastIndexOf(','), 1);
                keyString = keyString.Remove(keyString.LastIndexOf(','), 1);

                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {tableName} ({keyString}) VALUES({paramString});";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Adds a new row to the MySQL database table associated with the type T.
        /// The row data is provided as an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="value">The object representing the row data to be added.</param>
        /// <returns>True if the row was successfully added; otherwise, false.</returns>
        public static bool AddTableRow<T>(this MySqlConnection connection, T value)
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return AddTableRow<T>(connection, tableAttribute.Name, value);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Adds a new amount of rows to the MySQL database table with the specified name.
        /// The row data is provided as an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table to which the row will be added.</param>
        /// <param name="value">The object representing the row data to be added.</param>
        /// <returns>True if the row was successfully added; otherwise, false.</returns>
        public static bool AddTableRows<T>(this MySqlConnection connection, string tableName, List<T> values)
        {
            if (connection == null)
                return false;

            if (values == null)
                return false;

            if (values.Count == 0)
                return false;

            try
            {
                var schemaType = typeof(T);
                string paramString = string.Empty;
                string keyString = string.Empty;
                var properties = schemaType.GetProperties();

                foreach (var value in values)
                {
                    paramString += "(";
                    foreach (var prop in properties)
                    {
                        if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                            continue;

                        var memberAttribute = prop.GetCustomAttribute<SqlMemberAttribute>();
                        string propName = prop.Name;

                        if (memberAttribute != null)
                        {
                            if (memberAttribute.ShouldAutoIncrement)
                                continue;

                            if (!memberAttribute.ColumnName.IsNullOrEmpty())
                                propName = memberAttribute.ColumnName;
                        }

                        if (!keyString.Contains(propName))
                            keyString += $"{propName},";

                        if (prop.PropertyType == typeof(bool) || prop.PropertyType.IsEnum)
                            paramString += $"'{Convert.ToInt32(prop.GetValue(value))}',";
                        else if (prop.PropertyType == typeof(DateTime))
                            paramString += $"'{((DateTime)prop.GetValue(value)).ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
                        else if (prop.PropertyType == typeof(string))
                            paramString += $"'{ConvertIllegalCharsToSql((string)prop.GetValue(value))}',";
                        else
                            paramString += $"'{prop.GetValue(value)}',";
                    }
                    if (paramString.LastIndexOf(',') > 0)
                        paramString = paramString.Remove(paramString.LastIndexOf(','), 1);
                    paramString += "),";
                }

                if (paramString.LastIndexOf(',') > 0)
                    paramString = paramString.Remove(paramString.LastIndexOf(','), 1);
                if (keyString.LastIndexOf(',') > 0)
                    keyString = keyString.Remove(keyString.LastIndexOf(','), 1);

                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {tableName} ({keyString}) VALUES{paramString};";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Updates an existing row in the MySQL database table associated with the type T.
        /// The row data is provided as an object of type T, and the update is performed based on the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the table to update the row in.</param>
        /// <param name="newValue">The object representing the updated row data.</param>
        /// <param name="whereClause">The WHERE clause used to locate the row to update.</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <returns>True if the row was successfully updated; otherwise, false.</returns>
        public static bool UpdateTableRow<T>(this MySqlConnection connection, string tableName, T newValue, string whereClause, List<MySqlParameter> parameters)
        {
            if (connection == null)
                return false;

            if (whereClause.IsNullOrEmpty())
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }

            try
            {
                var schemaType = typeof(T);
                string setClause = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var memberAttribute = prop.GetCustomAttribute<SqlMemberAttribute>();
                    string propName = prop.Name;

                    if (memberAttribute != null)
                    {
                        if (memberAttribute.ShouldAutoIncrement)
                            continue;

                        if (!memberAttribute.ColumnName.IsNullOrEmpty())
                            propName = memberAttribute.ColumnName;
                    }

                    if (prop.PropertyType == typeof(bool) || prop.PropertyType.IsEnum)
                        setClause += $"{propName}={Convert.ToInt32(prop.GetValue(newValue))},";
                    else if (prop.PropertyType == typeof(DateTime))
                        setClause += $"{propName}='{((DateTime)prop.GetValue(newValue)).ToString("yyyy-MM-dd HH:mm:ss.fff")}',";
                    else if (prop.PropertyType == typeof(string))
                        setClause += $"{propName}='{ConvertIllegalCharsToSql((string)prop.GetValue(newValue))}',";
                    else
                        setClause += $"{propName}='{prop.GetValue(newValue)}',";
                }

                setClause = setClause.Remove(setClause.LastIndexOf(','), 1);
                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    if (parameters != null)
                        foreach (var parameter in parameters)
                            command.Parameters.Add(parameter);

                    if (!whereClause.IsNullOrEmpty())
                    {
                        if (whereClause.StartsWith("WHERE"))
                            whereClause = whereClause.Replace("WHERE", "");

                        if (whereClause.StartsWith(" WHERE"))
                            whereClause = whereClause.Replace(" WHERE", "");
                    }
                    command.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Updates an existing row in the MySQL database table associated with the type T.
        /// The row data is provided as an object of type T, and the update is performed based on the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="newValue">The object representing the updated row data.</param>
        /// <param name="whereClause">The WHERE clause used to locate the row to update.</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <returns>True if the row was successfully updated; otherwise, false.</returns>
        public static bool UpdateTableRow<T>(this MySqlConnection connection, T newValue, string whereClause, List<MySqlParameter> parameters)
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return UpdateTableRow<T>(connection, tableAttribute.Name, newValue, whereClause, parameters);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        public static bool UpdateTableRow<T>(this MySqlConnection connection, string tableName, string whereClause, Compatibility.Database.SqlParameter newValue)
        {
            if (connection == null)
                return false;

            if (newValue == null)
                return false;

            if (whereClause.IsNullOrEmpty())
                return false;

            try
            {
                var schemaType = typeof(T);
                string setClause = $"{newValue.ColumnName}={newValue.Value.ParameterName}";

                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.Add(newValue.Value);

                    if (whereClause.StartsWith("WHERE"))
                        whereClause = whereClause.Replace("WHERE", "");

                    if (whereClause.StartsWith(" WHERE"))
                        whereClause = whereClause.Replace(" WHERE", "");

                    command.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        public static bool UpdateTableRow<T>(this MySqlConnection connection, string whereClause, Compatibility.Database.SqlParameter newValue)
        {
            if (connection == null)
                return false;

            if (newValue == null)
                return false;

            if (whereClause.IsNullOrEmpty())
                return false;

            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return UpdateTableRow<T>(connection, tableName: tableAttribute.Name, whereClause: whereClause, newValue: newValue);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        public static bool UpdateTableRow<T>(this MySqlConnection connection, string tableName, string whereClause, List<Compatibility.Database.SqlParameter> newValues)
        {
            if (connection == null)
                return false;

            if (newValues == null)
                return false;

            if (newValues.Count == 0)
                return false;

            if (whereClause.IsNullOrEmpty())
                return false;

            try
            {
                var schemaType = typeof(T);
                string setClause = string.Empty;

                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    foreach (var par in newValues)
                    {
                        setClause += $"{par.ColumnName}={par.Value.ParameterName},";
                        command.Parameters.Add(par.Value);
                    }

                    setClause = setClause.Remove(setClause.LastIndexOf(','), 1);
                    if (whereClause.StartsWith("WHERE"))
                        whereClause = whereClause.Replace("WHERE", "");

                    if (whereClause.StartsWith(" WHERE"))
                        whereClause = whereClause.Replace(" WHERE", "");

                    command.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        public static bool UpdateTableRow<T>(this MySqlConnection connection, string whereClause, List<Compatibility.Database.SqlParameter> newValues)
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return UpdateTableRow<T>(connection, tableAttribute.Name, whereClause, newValues);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Removes a row from the MySQL database table associated with the type T.
        /// The row to remove is specified using the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="tableName">The name of the database table.</param>
        /// <param name="whereClause">The WHERE clause used to locate the row to remove.</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <returns>True if the row was successfully removed; otherwise, false.</returns>
        public static bool RemoveTableRow<T>(this MySqlConnection connection, string tableName, string whereClause, List<MySqlParameter> parameters)
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                connection.OpenSafe();
                using (var command = connection.CreateCommand())
                {
                    if (parameters != null)
                        foreach (var parameter in parameters)
                            command.Parameters.Add(parameter);

                    if (!whereClause.IsNullOrEmpty())
                    {
                        if (whereClause.StartsWith("WHERE"))
                            whereClause = whereClause.Replace("WHERE", "");

                        if (whereClause.StartsWith(" WHERE"))
                            whereClause = whereClause.Replace(" WHERE", "");
                    }
                    command.CommandText = $"DELETE FROM {tableName} WHERE {whereClause}";
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }

        /// <summary>
        /// Removes a row from the MySQL database table associated with the type T.
        /// The row to remove is specified using the provided WHERE clause and parameters.
        /// </summary>
        /// <typeparam name="T">The type of object associated with the table.</typeparam>
        /// <param name="connection">The MySqlConnection to the MySQL database.</param>
        /// <param name="whereClause">The WHERE clause used to locate the row to remove.</param>
        /// <param name="parameters">The list of MySqlParameters used in the WHERE clause.</param>
        /// <returns>True if the row was successfully removed; otherwise, false.</returns>
        public static bool RemoveTableRow<T>(this MySqlConnection connection, string whereClause, List<MySqlParameter> parameters)
        {
            if (connection == null)
                return false;

            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                return RemoveTableRow<T>(connection, tableAttribute.Name, whereClause, parameters);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return false;
            }
        }
    }
}
