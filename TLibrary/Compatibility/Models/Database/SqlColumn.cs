using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Extensions;
using UnityEngine;

namespace Tavstal.TLibrary.Compatibility.Database
{
    /// <summary>
    /// Class used to help in handling SQL columns
    /// </summary>
    public class SqlColumn
    {
        public string ColumnName { get; internal set; }
        public string ColumnType { get; internal set; }
        public bool IsNullable { get; internal set; }
        public bool IsUnsigned { get; internal set; }
        public bool ShouldAutoIncrement { get; internal set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsUnique { get; private set; }
        public string ForeignTable { get; private set; }
        public string ForeignColumn { get; private set; }
        public bool IsForeignKey { get; private set; }

        public SqlColumn()
        {

        }

        public SqlColumn(string columnName, string columnType, bool isNullable, bool shouldAutoIncrement, bool isPrimaryKey, bool isUnique, bool isUnsigned, string foreignTable, string foreignColumn, bool isForeignKey) 
        {
            ColumnName = columnName;
            ColumnType = columnType;
            IsNullable = isNullable;
            ShouldAutoIncrement = shouldAutoIncrement;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUnique;
            IsUnsigned = isUnsigned;
            ForeignTable = foreignTable;
            ForeignColumn = foreignColumn;
            IsForeignKey = isForeignKey;
        }

        public void SetAsPrimaryKey()
        {
            IsPrimaryKey = true;
            IsUnique = false;
        }

        public void SetAsUniqueKey()
        {
            IsPrimaryKey = false;
            IsUnique = true;
        }

        public void SetForeignKey(string tableName, string columnName)
        {
            if (tableName.IsNullOrEmpty() || columnName.IsNullOrEmpty())
            {
                IsForeignKey = false;
                return;
            }

            IsForeignKey = true;
            ForeignTable = tableName;
            ForeignColumn = columnName;
        }

        public static bool operator ==(SqlColumn column1, SqlColumn column2)
        {
            if (ReferenceEquals(column1, column2))
                return true;
            if (ReferenceEquals(column1, null))
                return false;
            if (ReferenceEquals(column2, null))
                return false;
            return column1.Equals(column2);
        }
        public static bool operator !=(SqlColumn column1, SqlColumn column2) => !(column1 == column2);
        public bool Equals(SqlColumn otherColumn)
        {
            if (ReferenceEquals(otherColumn, null))
                return false;
            if (ReferenceEquals(this, otherColumn))
                return true;

            return ColumnName == otherColumn.ColumnName &&
                   ColumnType == otherColumn.ColumnType &&
                   ShouldAutoIncrement == otherColumn.ShouldAutoIncrement &&
                   IsUnique == otherColumn.IsUnique &&
                   IsPrimaryKey == otherColumn.IsPrimaryKey &&
                   IsNullable == otherColumn.IsNullable &&
                   IsUnsigned == otherColumn.IsUnsigned;
        }
        public override bool Equals(object obj) => Equals(obj as SqlColumn);

        public override int GetHashCode()
        {
            int hashCode = 1349296267;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ColumnName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ColumnType);
            hashCode = hashCode * -1521134295 + IsNullable.GetHashCode();
            hashCode = hashCode * -1521134295 + IsUnsigned.GetHashCode();
            hashCode = hashCode * -1521134295 + ShouldAutoIncrement.GetHashCode();
            hashCode = hashCode * -1521134295 + IsPrimaryKey.GetHashCode();
            hashCode = hashCode * -1521134295 + IsUnique.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ForeignTable);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ForeignColumn);
            hashCode = hashCode * -1521134295 + IsForeignKey.GetHashCode();
            return hashCode;
        }
    }
}
