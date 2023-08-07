using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Database
{
    [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class SqlMemberAttribute : Attribute
    {
        public string ColumnName { get; internal set; }
        public string ColumnType { get; internal set; }
        public bool IsNullable { get; internal set; }
        public bool ShouldAutoIncrement { get; internal set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsUnique { get; private set; }
        public string ForeignTable { get; private set; }
        public string ForeignColumn { get; private set; }
        public bool IsForeignKey { get; private set; }

        public SqlMemberAttribute(string columnName = null, string columnType = null, bool isNullable = false, bool shouldAutoIncrement = false, bool isPrimaryKey = false, bool isUniqueKey = false, string foreignTable = null, string foreignColumn = null, bool isForeignKey = false)
        {
            ColumnName = columnName;
            ColumnType = columnType;
            IsNullable = isNullable;
            ShouldAutoIncrement = shouldAutoIncrement;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUniqueKey;
            ForeignTable = foreignTable;
            ForeignColumn = foreignColumn;
            IsForeignKey = isForeignKey;
        }
    
        public SqlColumn ToColumn()
        {
            return new SqlColumn(ColumnName, ColumnType, IsNullable, ShouldAutoIncrement, IsPrimaryKey, IsUnique, ForeignTable, ForeignColumn, IsForeignKey);
        }
    }
}
