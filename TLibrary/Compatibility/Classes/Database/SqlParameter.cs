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

            var info = expression.Parameters.ElementAt(0);
            Type type = info.Type;

            if (type.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                return null;

            var memberAttribute = type.GetCustomAttribute<SqlMemberAttribute>();
            string propName = type.Name;

            if (memberAttribute != null)
            {
                if (memberAttribute.ShouldAutoIncrement)
                    return null;

                if (!memberAttribute.ColumnName.IsNullOrEmpty())
                    propName = memberAttribute.ColumnName;

                if (!memberAttribute.IsNullable && value == null)
                    return null;
            }

            return new SqlParameter(propName, new MySqlParameter($"@{propName}", value: value));
        }
        
    }
}
