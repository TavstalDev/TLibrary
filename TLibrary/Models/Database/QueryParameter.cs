namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// A condition used in the WHERE clause of SQL queries (SELECT, DELETE, UPDATE).
    /// Built through the shorthand factories or the direct constructor.
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// The database column name to filter on.
        /// </summary>
        public string ColumnName { get; }
        
        /// <summary>
        /// The SQL comparison operator (e.g. <c>=</c>, <c>!=</c>, <c>LIKE</c>).
        /// </summary>
        public string Operator { get; }
        
        /// <summary>
        /// The value to compare against. <c>null</c> is treated as <see cref="System.DBNull.Value"/>.
        /// </summary>
        public object? Value { get; }
        
        /// <param name="columnName">The column name.</param>
        /// <param name="operator">The SQL operator.</param>
        /// <param name="value">The comparison value.</param>
        public QueryParameter(string columnName, string @operator, object? value)
        {
            ColumnName = columnName;
            Operator = @operator;
            Value = value;
        }

        public override string ToString() =>
            $"{ColumnName} {Operator} {Value}";

        /// <summary>Equal to (<c>=</c>).</summary>
        public static QueryParameter eq(string columnName, object? value) =>
            new QueryParameter(columnName, "=", value);
        
        /// <summary>Not equal to (<c>!=</c>).</summary>
        public static QueryParameter ne(string columnName, object? value) =>
            new QueryParameter(columnName, "!=", value);
        
        /// <summary>Not equal to (<c>!=</c>). Alias of <see cref="ne"/>.</summary>
        public static QueryParameter not(string columnName, object? value) =>
            new QueryParameter(columnName, "!=", value);
        
        /// <summary>Less than (<c>&lt;</c>).</summary>
        public static QueryParameter lt(string columnName, object? value) =>
            new QueryParameter(columnName, "<", value);
        
        /// <summary>Less than or equal (<c>&lt;=</c>).</summary>
        public static QueryParameter lte(string columnName, object? value) =>
            new QueryParameter(columnName, "<=", value);
        
        /// <summary>Greater than (<c>&gt;</c>).</summary>
        public static QueryParameter gt(string columnName, object? value) =>
            new QueryParameter(columnName, ">", value);
        
        /// <summary>Greater than or equal (<c>&gt;=</c>).</summary>
        public static QueryParameter gte(string columnName, object? value) =>
            new QueryParameter(columnName, ">=", value);
        
        /// <summary>SQL LIKE match.</summary>
        public static QueryParameter like(string columnName, object? value) =>
            new QueryParameter(columnName, "LIKE", value);
    }
}