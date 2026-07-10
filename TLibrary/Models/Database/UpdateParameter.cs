using System;
using System.Linq.Expressions;
using System.Reflection;
using MySqlConnector;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// A column-value pair used in parameterized SQL UPDATE statements.
    /// Created through the <see cref="Get{T}"/> factory method or the direct constructor.
    /// </summary>
    public class UpdateParameter
    {
        /// <summary>The database column name to update. Derived from the property name or from <see cref="SqlMemberAttribute.ColumnName"/>.</summary>
        public string ColumnName { get; }
        
        /// <summary>The parameterized MySQL value. When <c>null</c>, <see cref="DBNull.Value"/> is used instead.</summary>
        public MySqlParameter? Value { get; }
        
        /// <param name="columnName">The database column name.</param>
        /// <param name="value">The parameterized value, or <c>null</c>.</param>
        public UpdateParameter(string columnName, MySqlParameter? value)
        {
            ColumnName = columnName;
            Value = value;
        }
        
        /// <summary>
        /// Builds an <see cref="UpdateParameter"/> from a typed expression (e.g. <c>p => p.Score</c>).
        /// Resolves the column name, respects <see cref="SqlIgnoreAttribute"/> and <see cref="SqlMemberAttribute"/>,
        /// and rejects auto-increment or non-nullable columns that receive <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">A lambda pointing to the property or field to update.</param>
        /// <param name="value">The new value for the column.</param>
        public static UpdateParameter Get<T>(Expression<Func<T, object>> expression, object? value) where T : class
        {
            MemberExpression memberExpression = expression.Body switch
            {
                MemberExpression memExpr => memExpr,
                UnaryExpression { Operand: MemberExpression memExpr } => memExpr,
                _ => throw new ArgumentException("Expression must point to a valid property or field.", nameof(expression))
            };

            MemberInfo member = memberExpression.Member;
            string propertyName = member.Name;

            if (member.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                throw new Exception("This property cannot be used because it has the SqlIgnoreAttribute attribute.");

            var memberAttribute = member.GetCustomAttribute<SqlMemberAttribute>();
            if (memberAttribute != null)
            {
                if (memberAttribute.ShouldAutoIncrement)
                   throw new Exception("This property cannot be used because it has automatic Increment enabled.");

                if (!string.IsNullOrEmpty(memberAttribute.ColumnName))
                    propertyName = memberAttribute.ColumnName!;

                if (!memberAttribute.IsNullable && value == null)
                    throw new Exception("The project is not nullable, but the provided value is null.");
            }

            return new UpdateParameter(propertyName, new MySqlParameter($"@{propertyName}", value ?? DBNull.Value));
        }
    }
}