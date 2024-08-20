using System;
using System.Linq.Expressions;
using System.Reflection;
using MySql.Data.MySqlClient;
using Tavstal.TLibrary.Compatibility.Models.Database.Attributes;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TLibrary.Compatibility.Models.Database
{
    /// <summary>
    /// Used to handle column value more easily
    /// </summary>
    public class SqlParameter
    {
        /// <summary>
        /// Name of the column
        /// </summary>
        public string ColumnName { get; internal set; }
        /// <summary>
        /// A <see cref="MySqlParameter"/> used as value
        /// </summary>
        public MySqlParameter Value { get; internal set; }

        internal SqlParameter(string column, MySqlParameter param)
        {
            ColumnName = column;
            Value = param;
        }

        public static SqlParameter Get<T>(Expression<Func<T, object>> expression, object value) where T : class
        {
            if (expression == null)
                return null;

            MemberExpression memberExpression = null;

            if (expression.Body is MemberExpression expression1)
            {
                memberExpression = expression1;
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression expression2)
            {
                memberExpression = expression2;
            }

            if (memberExpression == null)
                throw new ArgumentException("Invalid expression");

            Type propertyType = memberExpression.Member.GetType();
            string propertyName = memberExpression.Member.Name;

            if (propertyType.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                return null;

            var memberAttribute = propertyType.GetCustomAttribute<SqlMemberAttribute>();

            if (memberAttribute != null)
            {
                if (memberAttribute.ShouldAutoIncrement)
                    return null;

                if (!memberAttribute.ColumnName.IsNullOrEmpty())
                    propertyName = memberAttribute.ColumnName;

                if (!memberAttribute.IsNullable && value == null)
                    return null;
            }

            return new SqlParameter(propertyName, new MySqlParameter($"@{propertyName}", value: value));
        }
        
    }
}
