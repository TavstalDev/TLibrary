using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TLibrary.Helpers.Database
{
    /// <summary>
    /// Provides helper methods to work with SQL data types and schema generation.
    /// </summary>
    public static class SqlTypeHelper
    {
        /// <summary>
        /// Generates the column and key definitions for a CREATE TABLE statement
        /// based on the properties of the given schema type.
        /// </summary>
        /// <param name="schemaType">The type that defines the table schema.</param>
        /// <returns>A string with column definitions and constraints, separated by commas.</returns>
        public static string GetSchemaCreateParams(Type schemaType)
        {
            List<string> paramList = new List<string>();
            List<string> keyParamList = new List<string>();
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
                if (sqlMember == null)
                    continue;

                if (!string.IsNullOrEmpty(sqlMember.ColumnName))
                    propName = sqlMember.ColumnName!;

                if (!string.IsNullOrEmpty(sqlMember.ColumnType))
                    typeName = sqlMember.ColumnType!;

                if (sqlMember.ShouldAutoIncrement)
                    autoincrementString = $" AUTO_INCREMENT";

                if (!sqlMember.IsNullable)
                    nullableString = $" NOT NULL";

                if (sqlMember.IsUnsigned)
                    nullableString = $" UNSIGNED";

                if (sqlMember.IsPrimaryKey)
                    keyParamList.Add($"PRIMARY KEY({propName})");
                else if (sqlMember.IsUnique)
                    keyParamList.Add($"UNIQUE ({propName})");

                if (sqlMember.IsForeignKey)
                    keyParamList.Add(
                        $"FOREIGN KEY ({propName}) REFERENCES {sqlMember.ForeignTable}({sqlMember.ForeignColumn})");

                paramList.Add($"{propName} {typeName}{unsignedString}{nullableString}{autoincrementString}");
            }

            paramList.AddRange(keyParamList);
            return string.Join(", ", paramList.ToArray());
        }

        /// <summary>
        /// Converts a C# data type to its corresponding SQL data type.
        /// </summary>
        /// <param name="type">The C# type to convert.</param>
        /// <param name="length">Optional length for types like INT or VARCHAR.</param>
        /// <returns>The SQL data type as a string.</returns>
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
                {
                    if (type == typeof(Guid))
                        return "LONGTEXT";
                    if (type == typeof(byte[]))
                        return "VARBINARY(MAX)";
                    throw new NotSupportedException($"Data type '{type.FullName}' is not supported.");
                }
            }
        }
        
        /// <summary>
        /// Converts an SQL data type string back to its corresponding C# type.
        /// </summary>
        /// <param name="sqlDataType">The SQL data type as a string.</param>
        /// <returns>The equivalent C# type.</returns>
        public static Type ConvertSqlToCSharpDataType(string sqlDataType)
        {
            if (sqlDataType == "TINYINT(1)")
                sqlDataType = "BOOL";

            if (sqlDataType.Contains("("))
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
                case "DOUBLE":
                    return typeof(double);
                case "DECIMAL":
                    return typeof(decimal);
                case "CHAR":
                case "VARCHAR":
                    return typeof(string);
                case "DATETIME":
                    return typeof(DateTime);
                case "LONGTEXT":
                    return typeof(Guid);
                case "TEXT":
                    return typeof(string);
                case "VARBINARY":
                    return typeof(byte[]);
                default:
                    throw new NotSupportedException($"SQL data type '{sqlDataType}' is not supported.");
            }
        }
        
        /// <summary>
        /// Replaces single quotes with a safe placeholder to prevent SQL injection.
        /// </summary>
        /// <param name="value">The input string that may contain illegal characters.</param>
        /// <returns>A string with single quotes replaced by "U+0027".</returns>
        public static string ConvertIllegalCharsToSql(string value) =>
            value.Replace("'", "U+0027");
        
        /// <summary>
        /// Restores the original single quotes from the safe placeholder.
        /// </summary>
        /// <param name="value">The string with "U+0027" placeholders.</param>
        /// <returns>A string with single quotes restored.</returns>
        public static string ConvertSqlToIllegalChars(string value) =>
            value.Replace("U+0027", "'");
    }
}