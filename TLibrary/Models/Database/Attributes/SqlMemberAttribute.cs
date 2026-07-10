using System;

namespace Tavstal.TLibrary.Models.Database.Attributes
{
    /// <summary>
    /// SQL Attribute used to declare variables as sql members, so the database manager will easily recognize it
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class SqlMemberAttribute : Attribute
    {
        public string? ColumnName { get; internal set; }
        public string? ColumnType { get; internal set; }
        public bool IsNullable { get; internal set; }
        public bool IsUnsigned { get; internal set; }
        public bool ShouldAutoIncrement { get; internal set; }
        public bool IsPrimaryKey { get; private set; }
        public bool IsUnique { get; private set; }
        public string? ForeignTable { get; private set; }
        public string? ForeignColumn { get; private set; }
        public bool IsForeignKey { get; private set; }

        public SqlMemberAttribute(string? columnName = null, string? columnType = null, bool isNullable = false, bool shouldAutoIncrement = false, bool isPrimaryKey = false, bool isUniqueKey = false, bool isUnsigned = false, string? foreignTable = null, string? foreignColumn = null, bool isForeignKey = false)
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
            IsUnsigned = isUnsigned;
        }
    }
}
