﻿using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Extensions;
using UnityEngine;

namespace Tavstal.TLibrary.Compatibility.Classes.Database
{
    public class SqlColumn
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

        public SqlColumn()
        {

        }

        public SqlColumn(string columnName, string columnType, bool isNullable, bool shouldAutoIncrement, bool isPrimaryKey, bool isUnique, string foreignTable, string foreignColumn, bool isForeignKey) 
        {
            ColumnName = columnName;
            ColumnType = columnType;
            IsNullable = isNullable;
            ShouldAutoIncrement = shouldAutoIncrement;
            IsPrimaryKey = isPrimaryKey;
            IsUnique = isUnique;
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
                   IsNullable == otherColumn.IsNullable;
        }
        public override bool Equals(object obj) => Equals(obj as SqlColumn);
    }
}
