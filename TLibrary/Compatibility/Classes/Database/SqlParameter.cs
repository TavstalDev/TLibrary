using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers;
using UnityEngine;

namespace Tavstal.TLibrary.Compatibility.Database
{
    public class SqlParameter
    {
        public string ColumnName { get; internal set; }
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

            if (expression.Body is MemberExpression)
            {
                memberExpression = (MemberExpression)expression.Body;
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
            {
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }

            if (memberExpression == null)
                throw new ArgumentException("Invalid expression");

            var type = typeof(T);

            Type propertyType = memberExpression.Member.GetType();
            LoggerHelper.LogWarning($"Property type: {propertyType.Name} - {propertyType.FullName}");
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
