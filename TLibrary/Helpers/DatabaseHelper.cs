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

        public static void CreateTable<T>(this MySqlConnection connection, T schemaObj)
        {
            try
            {
                var schemaType = schemaObj.GetType();
                var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                var command = connection.CreateCommand();
                command.CommandText = $"SHOW TABLES LIKE {attribute.Name}";
                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null)
                    return;

                string schemaParams = string.Empty;
                string keyParams = string.Empty;

                foreach (var prop in schemaType.GetProperties())
                {
                    var propAttribute = prop.GetCustomAttribute<SqlNameAttribute>();
                    string propName = propAttribute == null ? prop.Name : propAttribute.Name;

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

                    schemaParams += $"{propName} {ConvertToSqlDataType(prop.PropertyType)}{nullableString},";
                }

                command.CommandText = $"CREATE TABLE {attribute.Name} ({schemaParams}{keyParams})";
                command.CommandText = command.CommandText.Remove(command.CommandText.LastIndexOf(','), 1);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
            }
        }

        public static void AddTableRow<T>(this MySqlConnection connection, T schemaObj)
        {
            try
            {
                var schemaType = schemaObj.GetType();
                var attribute = schemaType.GetCustomAttribute<SqlNameAttribute>();
                if (attribute == null)
                    throw new ArgumentNullException("The given schemaObj does not have SqlNameAttribute.");

                MySqlCommand MySQLCommand = connection.CreateCommand();
                connection.Open();

                MySQLCommand.CommandText = $"SELECT * FROM {attribute.Name} WHERE ";
                object data = MySQLCommand.ExecuteScalar();

                if (data == null)
                    return;
                MySQLCommand.CommandText = $"INSERT INTO {attribute.Name} () VALUES ()";
                MySQLCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException("Error in TLibrary:");
                LoggerHelper.LogError(ex);
            }
        }

        public static void FillParametersFromObject<T>(this MySqlCommand command, T obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                Console.WriteLine($"FILL PROPERTY DEBUG: @{property.Name.Normalize()}, Value: {property.GetValue(obj)}");
                command.Parameters.AddWithValue($"@{property.Name.Normalize()}", property.GetValue(obj));
            }

            foreach (var field in obj.GetType().GetFields())
            {
                Console.WriteLine($"FILL FIELD DEBUG: @{field.Name.Normalize()}, Value: {field.GetValue(obj)}");
                command.Parameters.AddWithValue($"@{field.Name.Normalize()}", field.GetValue(obj));
            }
        }

        public static void GenerateUpdateCommand<T>(this MySqlCommand command, string tablename, T value)
        {
            string paramString = string.Empty;
            string conditionString = string.Empty;

            command.CommandText = $"UPDATE {tablename} SET {paramString} WHERE {conditionString}";
        }
    }
}
