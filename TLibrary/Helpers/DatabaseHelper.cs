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
using Tavstal.TLibrary.Compatibility.Classes.Database;
using Tavstal.TLibrary.Extensions;
using SDG.Unturned;
using UnityEngine;

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
                    return "BIT";
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
                    return length.HasValue ? $"VARCHAR({length.Value})" : "VARCHAR";
                case TypeCode.DateTime:
                    return "DATETIME";
                default:
                    if (type == typeof(Guid))
                    {
                        return "UNIQUEIDENTIFIER";
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
            if (sqlDataType.Contains('('))
                sqlDataType = sqlDataType.Split('(')[0];

            switch (sqlDataType.ToUpper())
            {
                case "BIT":
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
                case "UNIQUEIDENTIFIER":
                    return typeof(Guid);
                case "VARBINARY":
                    return typeof(byte[]);
                default:
                    throw new NotSupportedException($"SQL data type '{sqlDataType}' is not supported.");
            }
        }

        /// <summary>
        /// Converts the current row in the MySqlDataReader to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to be converted.</typeparam>
        /// <param name="reader">The MySqlDataReader to read data from.</param>
        /// <returns>An object of the specified type representing the current row data, or default(T) if the conversion fails.</returns>
        public static T ConvertToObject<T>(this MySqlDataReader reader) where T : class
        {
            if (!reader.HasRows)
                return default(T);

            T obj = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (reader[property.Name] != DBNull.Value)
                {
                    object value = reader[property.Name];
                    property.SetValue(obj, value);
                }
            }

            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                if (reader[field.Name] != DBNull.Value)
                {
                    object value = reader[field.Name];
                    field.SetValue(obj, value);
                }
            }

            return obj;
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

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SHOW TABLES LIKE {tableName}";
                    object result = command.ExecuteScalar();
                    if (result != null)
                        return false;
                }

                string schemaParams = string.Empty;
                string keyParams = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;
                    string typeName = ConvertToSqlDataType(prop.PropertyType);

                    var sqlFieldType = prop.GetCustomAttribute<SqlFieldTypeAttribute>();
                    if (sqlFieldType != null)
                        typeName = sqlFieldType.Type;

                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string nullableString = string.Empty;
                    if (prop.GetCustomAttribute<SqlNonNullableAttribute>() != null)
                        nullableString = $" NOT NULL";

                    if (prop.GetCustomAttribute<SqlPrimaryKeyAttribute>() != null)
                        keyParams += $"PRIMARY KEY({propName}),";

                    if (prop.GetCustomAttribute<SqlUniqueKeyAttribute>() != null)
                        keyParams += $"UNIQUE ({propName}),";

                    var foreignKey = prop.GetCustomAttribute<SqlForeignKeyAttribute>();
                    if (foreignKey != null)
                        keyParams += $"FOREIGN KEY ({propName}) REFERENCES {foreignKey.TableName}({foreignKey.TableColumn}),";

                    schemaParams += $"{propName} {typeName}{nullableString},";
                }

                foreach (var prop in schemaType.GetFields())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;
                    string typeName = ConvertToSqlDataType(prop.FieldType);

                    var sqlFieldType = prop.GetCustomAttribute<SqlFieldTypeAttribute>();
                    if (sqlFieldType != null)
                        typeName = sqlFieldType.Type;

                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    string nullableString = string.Empty;
                    if (prop.GetCustomAttribute<SqlNonNullableAttribute>() != null)
                        nullableString = $" NOT NULL";

                    if (prop.GetCustomAttribute<SqlPrimaryKeyAttribute>() != null)
                        keyParams += $"PRIMARY KEY({propName}),";

                    if (prop.GetCustomAttribute<SqlUniqueKeyAttribute>() != null)
                        keyParams += $"UNIQUE ({propName}),";

                    var foreignKey = prop.GetCustomAttribute<SqlForeignKeyAttribute>();
                    if (foreignKey != null)
                        keyParams += $"FOREIGN KEY ({propName}) REFERENCES {foreignKey.TableName}({foreignKey.TableColumn}),";

                    schemaParams += $"{propName} {typeName}{nullableString},";
                }

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

                    SqlColumn localColumn = new SqlColumn();
                    // Column Name
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    localColumn.ColumnName = propAttribute == null ? prop.Name : propAttribute.Name;

                    // Column Type
                    string typeName = ConvertToSqlDataType(prop.PropertyType);
                    var sqlFieldType = prop.GetCustomAttribute<SqlFieldTypeAttribute>();
                    if (sqlFieldType != null)
                        typeName = sqlFieldType.Type;
                    localColumn.ColumnType = typeName;

                    // Column Is Nullable
                    if (prop.GetCustomAttribute<SqlNonNullableAttribute>() != null)
                        localColumn.IsNullable = false;
                    else
                        localColumn.IsNullable = true;

                    // Colum Primary Key
                    if (prop.GetCustomAttribute<SqlPrimaryKeyAttribute>() != null)
                        localColumn.SetAsPrimaryKey();

                    // Column Unique Key
                    if (prop.GetCustomAttribute<SqlUniqueKeyAttribute>() != null)
                        localColumn.SetAsUniqueKey();

                    // Column Foreign Key
                    var foreignKey = prop.GetCustomAttribute<SqlForeignKeyAttribute>();
                    if (foreignKey != null)
                        localColumn.SetForeignKey(foreignKey.TableName, foreignKey.TableColumn);

                    classColumns.Add(localColumn);
                }

                foreach (var prop in schemaType.GetFields())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    SqlColumn localColumn = new SqlColumn();
                    // Column Name
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    localColumn.ColumnName = propAttribute == null ? prop.Name : propAttribute.Name;

                    // Column Type
                    string typeName = ConvertToSqlDataType(prop.FieldType);
                    var sqlFieldType = prop.GetCustomAttribute<SqlFieldTypeAttribute>();
                    if (sqlFieldType != null)
                        typeName = sqlFieldType.Type;
                    localColumn.ColumnType = typeName;

                    // Column Is Nullable
                    if (prop.GetCustomAttribute<SqlNonNullableAttribute>() != null)
                        localColumn.IsNullable = false;
                    else
                        localColumn.IsNullable = true;

                    // Colum Primary Key
                    if (prop.GetCustomAttribute<SqlPrimaryKeyAttribute>() != null)
                        localColumn.SetAsPrimaryKey();

                    // Column Unique Key
                    if (prop.GetCustomAttribute<SqlUniqueKeyAttribute>() != null)
                        localColumn.SetAsUniqueKey();

                    // Column Foreign Key
                    var foreignKey = prop.GetCustomAttribute<SqlForeignKeyAttribute>();
                    if (foreignKey != null)
                        localColumn.SetForeignKey(foreignKey.TableName, foreignKey.TableColumn);

                    classColumns.Add(localColumn);

                }
                #endregion

                connection.Open();
                List<SqlColumn> liveColumns = new List<SqlColumn>();
                List<SqlColumn> columnsToRemove = new List<SqlColumn>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SHOW TABLES LIKE {tableName}";

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

                            var localColumn = new SqlColumn(
                                columnName: reader.GetString("Field"),
                                columnType: reader.GetString("Type"),
                                isNullable: reader.GetBoolean("Null"),
                                isPrimaryKey: columnKey == "PRI",
                                isUnique: columnKey == "UNI",
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
                    if (!liveColumns.Any(x => x.ColumnName == column.ColumnName))
                    {
                        columnsToAdd.Add(column);
                        continue;
                    }

                    if (liveColumns.Any(x => x.ColumnName == column.ColumnName && x != column))
                    {
                        columnsToUpdate.Add(column);
                        continue;
                    }
                }
                #endregion

                #region Modify Table 
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"ALTER TABLE {tableName}";
                    // ADD
                    foreach (SqlColumn column in columnsToAdd)
                    {
                        string primaryKey = column.IsPrimaryKey ? " PRIMARY KEY" : "";
                        string nullable = column.IsNullable ? "" : " NOT NULL";
                        string uniqueKey = column.IsUnique ? " UNIQUE" : "";
                        string autoIncrement = column.ShouldAutoIncrement ? " AUTO_INCREMENT" : "";
                        command.CommandText += $" ADD {column.ColumnName}{nullable} {column.ColumnType}{uniqueKey}{primaryKey}{autoIncrement},";
                    }
                    // UPDATE
                    foreach (SqlColumn column in columnsToUpdate)
                    {
                        string primaryKey = column.IsPrimaryKey ? " PRIMARY KEY" : "";
                        string nullable = column.IsNullable ? "" : " NOT NULL";
                        string uniqueKey = column.IsUnique ? " UNIQUE" : "";
                        string autoIncrement = column.ShouldAutoIncrement ? " AUTO_INCREMENT" : "";
                        command.CommandText += $" ALTER COLUMN {column.ColumnName}{nullable} {column.ColumnType}{uniqueKey}{primaryKey}{autoIncrement},";
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
                connection.Open();
                string limitClause = limit > 0 ? $" LIMIT: {limit}" : "";
                string whereClauseExp = string.Empty;
                if (!whereClause.IsNullOrEmpty())
                {
                    if (whereClause.StartsWith("WHERE"))
                        whereClause = " " + whereClause;

                    if (!whereClause.StartsWith(" WHERE"))
                        whereClause = " WHERE" + whereClause;
                }
                using (var command = connection.CreateCommand())
                {
                    if (parameters != null)
                        foreach (var param in parameters)
                            command.Parameters.Add(param);
                    command.CommandText = $"SELECT * FROM {tableName}{whereClause}{limitClause}";
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        localList.Add(ConvertToObject<T>(reader));
                    }
                }

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

                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    keyString += $"{propName},";
                    paramString += $"`{prop.GetValue(value)}`,";
                }

                foreach (var prop in schemaType.GetFields())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    keyString += $"{propName},";
                    paramString += $"`{prop.GetValue(value)}`,";
                }

                paramString = paramString.Remove(paramString.LastIndexOf(','), 1);
                keyString = keyString.Remove(keyString.LastIndexOf(','), 1);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO {tableName} ({keyString}) VALUES ({paramString})";
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

            try
            {
                var schemaType = typeof(T);
                string setClause = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    setClause += $"{propName}={prop.GetValue(newValue)},";
                }

                foreach (var prop in schemaType.GetFields())
                {
                    if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                        continue;

                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    setClause += $"{propName}={prop.GetValue(newValue)},";
                }

                setClause = setClause.Remove(setClause.LastIndexOf(','), 1);
                using (var command = connection.CreateCommand())
                {
                    if (parameters != null)
                        foreach (var parameter in parameters)
                            command.Parameters.Add(parameter);
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
                using (var command = connection.CreateCommand())
                {
                    if (parameters != null)
                        foreach (var parameter in parameters)
                            command.Parameters.Add(parameter);
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
