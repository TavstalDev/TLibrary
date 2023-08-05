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

namespace Tavstal.TLibrary.Helpers
{
    public static class DatabaseHelper
    {
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

        public static T ConvertToObject<T>(this MySqlDataReader reader)
        {
            if (!reader.HasRows)
                return default(T); // Return default value for the type T

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

        public static bool CreateTable<T>(this MySqlConnection connection)
        {
            try
            {
                var schemaType = typeof(T);
                var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                var command = connection.CreateCommand();
                command.CommandText = $"SHOW TABLES LIKE {attribute.Name}";
                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                    return false;

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

                command.CommandText = $"CREATE TABLE {attribute.Name} ({schemaParams}{keyParams})";
                command.CommandText = command.CommandText.Remove(command.CommandText.LastIndexOf(','), 1);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return false;
            }
        }

        public static bool CompareTable<T>(this MySqlConnection connection)
        {
            var schemaType = typeof(T);
            var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
            if (attribute == null)
                throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

            var command = connection.CreateCommand();
            command.CommandText = $"SHOW TABLES LIKE {attribute.Name}";
            connection.Open();

            object result = command.ExecuteScalar();
            if (result == null)
                return false;
        }

        /*public static void AddTableRow<T>(this MySqlConnection connection, T value)
        {
            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                MySqlCommand MySQLCommand = connection.CreateCommand();

                string paramString = string.Empty;
                string keyString = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    keyString += $"{propName},";
                    paramString += $"`{prop.GetValue(value)}`,";
                }

                foreach (var prop in schemaType.GetFields())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    keyString += $"{propName},";
                    paramString += $"`{prop.GetValue(value)}`,";
                }

                paramString = paramString.Remove(paramString.LastIndexOf(','), 1);
                keyString = keyString.Remove(keyString.LastIndexOf(','), 1);

                MySQLCommand.CommandText = $"INSERT INTO {tableAttribute.Name} ({keyString}) VALUES ({paramString})";
                MySQLCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
            }
        }

        public static void UpdateTableRow<T>(this MySqlConnection connection, T newvalue, Expression<Func<T, object>> predicate)
        {
            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                MySqlCommand MySQLCommand = connection.CreateCommand();

                string paramString = string.Empty;
                string keyString = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    keyString += $"{propName},";
                    paramString += $"`{prop.GetValue(newvalue)}`,";
                }

                foreach (var prop in schemaType.GetFields())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

                    keyString += $"{propName},";
                    paramString += $"`{prop.GetValue(newvalue)}`,";
                }

                paramString = paramString.Remove(paramString.LastIndexOf(','), 1);
                keyString = keyString.Remove(keyString.LastIndexOf(','), 1);

                MySQLCommand.CommandText = $"UPDATE {tableAttribute.Name} SET WHERE";
                MySQLCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
            }
        }

        public static T GetTableRow<T>(this MySqlConnection connection, Expression<Func<T, object>> predicate)
        {
            try
            {
                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                if (predicate == null)
                    throw new ArgumentNullException("The given predicate was null.");

                MySqlCommand MySQLCommand = connection.CreateCommand();

                var expressions = LinqHelper.GetExpressions((BinaryExpression)predicate.Body);
                string searchString = string.Empty;
                foreach (var expression in expressions)
                {
                    var memberName = LinqHelper.GetMemberName(expression);
                    var value = Expression.Lambda(expression).Compile().DynamicInvoke();
                    searchString += $"{memberName}=`{value}`, ";
                }
                searchString = searchString.Remove(searchString.LastIndexOf(','), 1);

                MySQLCommand.CommandText = $"SELECT * FROM {tableAttribute.Name} WHERE {searchString};";
                MySqlDataReader Reader = MySQLCommand.ExecuteReader();

                if (Reader == null)
                    return default;

                T val = default;
                while (Reader.Read())
                {
                    val = Reader.ConvertToObject<T>();
                    break;
                }
                Reader.Close();
                MySQLCommand.ExecuteNonQuery();
                return val;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }

        public static List<T> GetTableRowList<T>(this MySqlConnection connection, Expression<Func<T, object>> predicate = null)
        {
            try
            {

                var schemaType = typeof(T);
                var tableAttribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (tableAttribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                MySqlCommand MySQLCommand = connection.CreateCommand();

                string searchString = string.Empty;
                if (predicate != null)
                {
                    var expressions = LinqHelper.GetExpressions((BinaryExpression)predicate.Body);
                    
                    foreach (var expression in expressions)
                    {
                        var memberName = LinqHelper.GetMemberName(expression);
                        var value = Expression.Lambda(expression).Compile().DynamicInvoke();
                        searchString += $"{memberName}=`{value}`, ";
                    }
                    searchString = " WHERE " + searchString.Remove(searchString.LastIndexOf(','), 1);
                }

                MySQLCommand.CommandText = $"SELECT * FROM {tableAttribute.Name}{searchString};";
                MySqlDataReader Reader = MySQLCommand.ExecuteReader();

                if (Reader == null)
                    return default;

                List<T> val = new List<T>();
                while (Reader.Read())
                {
                    val.Add(Reader.ConvertToObject<T>());
                }
                Reader.Close();
                MySQLCommand.ExecuteNonQuery();
                return val;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
                return default;
            }
        }*/
    }
}
