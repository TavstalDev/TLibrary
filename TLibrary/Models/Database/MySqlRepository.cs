using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MySqlConnector;
using Tavstal.TLibrary.Extensions.Database;
using Tavstal.TLibrary.Helpers.Database;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TLibrary.Models.Database
{
    /// <summary>
    /// Provides a generic MySQL repository for performing CRUD operations on entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="ID">The type of the primary key identifier.</typeparam>
    /// <typeparam name="T">The entity type to operate on, which must be a reference type.</typeparam>
    public class MySqlRepository<ID, T> where T : class
    {
        protected readonly string _tableName;
        protected readonly string _idColumnName;
        protected readonly Type _classType;
        protected readonly IDatabaseManager _databaseManager;
        protected readonly Dictionary<PropertyInfo, string> _columnMappings = new Dictionary<PropertyInfo, string>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlRepository{ID, T}"/> class.
        /// </summary>
        /// <param name="databaseManager">The database manager used to create connections.</param>
        /// <param name="prefix">The prefix to prepend to the table name.</param>
        /// <exception cref="Exception">Thrown when the entity type <typeparamref name="T"/> has no primary column declared.</exception>
        public MySqlRepository(IDatabaseManager databaseManager, string prefix)
        {
            _databaseManager = databaseManager;
            _classType = typeof(T);
            _tableName = _classType.Name;
            var nameAttribute = _classType.GetCustomAttribute<SqlNameAttribute>();
            if (nameAttribute != null)
                _tableName = nameAttribute.Name;
            _tableName =  prefix + _tableName;

            foreach (var prop in _classType.GetProperties())
            {
                if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                    continue;

                string propName = prop.Name;
                var sqlMember = prop.GetCustomAttribute<SqlMemberAttribute>();
                if (sqlMember != null)
                {
                    if (!string.IsNullOrEmpty(sqlMember.ColumnName))
                        propName = sqlMember.ColumnName!;
                    if (sqlMember.IsPrimaryKey)
                        _idColumnName = propName;
                }
                
                _columnMappings[prop] = propName;
            }
            
            if (_idColumnName == null)
                throw new Exception($"{_classType.Name} has no primary column declared.");
        }
        
        /// <summary>
        /// Ensures the database table exists, creating it if necessary.
        /// </summary>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        public async Task CheckSchemaAsync(MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return;
                
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                string schemaParams = SqlTypeHelper.GetSchemaCreateParams(_classType);
                command.CommandText = $"CREATE TABLE IF NOT EXISTS `{_tableName}` ({schemaParams});";
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                HandleException(nameof(CheckSchemaAsync), ex);
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Inserts a new entity into the database table.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <returns>The inserted entity with its generated primary key, or <see langword="null"/> if the operation failed.</returns>
        public async Task<T?> AddAsync(T entity, MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return null;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return null;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                List<string> columnList = new  List<string>();
                List<string> columnValues = new  List<string>();
                int paramIndex = 0;
                foreach (var columnMap in _columnMappings)
                {
                    var fieldInfo = columnMap.Key;
                    string column = columnMap.Value;
                    
                    columnList.Add($"`{column}`");
                    string paramName = $"@p{paramIndex++}";
                    columnValues.Add(paramName);
                    
                    var value = fieldInfo.GetValue(entity);
                    command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(value));
                }
                
                string columns =  string.Join(", ", columnList);
                string values =  string.Join(", ", columnValues);
                command.CommandText = $"INSERT INTO `{_tableName}` ({columns}) VALUES ({values});" +
                                      $"SELECT * FROM `{_tableName}` WHERE `{_idColumnName}` = LAST_INSERT_ID();";

                T? result = null;
                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    result = reader.ConvertToObject<T>();
                return result;
            }
            catch (Exception ex)
            {
                HandleException(nameof(AddAsync), ex);
                return null;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }
        
        /// <summary>
        /// Inserts multiple entities into the database table in a single batch operation.
        /// </summary>
        /// <param name="entities">The list of entities to insert.</param>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <returns><see langword="true"/> if at least one row was inserted; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> AddRangeAsync(List<T> entities, MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return false;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return false;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                var columnNames = _columnMappings.Values.Select(c => $"`{c}`").ToList();
                var valueGroups = new List<string>();
                int paramIndex = 0;
                
                foreach (var entity in entities)
                {
                    var currentParams = new List<string>();
                    foreach (var columnMap in _columnMappings)
                    {
                        string paramName = $"@p{paramIndex++}";
                        currentParams.Add(paramName);
                
                        var value = columnMap.Key.GetValue(entity);
                        command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(value));
                    }
                    valueGroups.Add($"({string.Join(", ", currentParams)})");
                }
                
                command.CommandText = $"INSERT INTO `{_tableName}` ({string.Join(", ", columnNames)}) VALUES {string.Join(", ", valueGroups)};";
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(AddAsync), ex);
                return false;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Updates an existing entity in the database by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to update.</param>
        /// <param name="entity">The entity containing the updated values.</param>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <returns><see langword="true"/> if the update was successful; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> UpdateAsync(ID id, T entity, MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return false;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return false;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                List<string> columnValues = new  List<string>();
                int paramIndex = 0;
                command.Parameters.AddWithValue("@Id", id);
                foreach (var columnMap in _columnMappings)
                {
                    var fieldInfo = columnMap.Key;
                    string column = columnMap.Value;
                    
                    if (column == _idColumnName)
                        continue;
                    
                    string paramName = $"@p{paramIndex++}";
                    columnValues.Add($"`{column}` = {paramName}");
                    
                    var value = fieldInfo.GetValue(entity);
                    command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(value));
                }
                
                string values =  string.Join(", ", columnValues);
                command.CommandText = $"UPDATE `{_tableName}` SET {values} WHERE `{_idColumnName}` = @Id;";
                
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(UpdateAsync), ex);
                return false;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Updates entities matching the query parameters with the specified column values.
        /// </summary>
        /// <param name="updateParameters">The columns and values to update.</param>
        /// <param name="queryParameters">The conditions to match for the update.</param>
        /// <returns><see langword="true"/> if at least one row was updated; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> UpdateAsync(List<UpdateParameter> updateParameters, params QueryParameter[] queryParameters) =>
            await UpdateAsync( null, null, updateParameters, queryParameters);

        /// <summary>
        /// Updates entities matching the query parameters with the specified column values.
        /// </summary>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <param name="updateParameters">The columns and values to update.</param>
        /// <param name="queryParameters">The conditions to match for the update.</param>
        /// <returns><see langword="true"/> if at least one row was updated; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> UpdateAsync(MySqlConnection? connection, MySqlTransaction? transaction, List<UpdateParameter> updateParameters, params QueryParameter[] queryParameters)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null || updateParameters.Count == 0 || queryParameters.Length == 0)
                return false;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return false;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                List<string> columnValues = new  List<string>();
                List<string> queryList = new  List<string>();
                int qParamIndex = 0;
                foreach (var column in _columnMappings.Values)
                {
                    if (column == _idColumnName)
                        continue;
                    
                    UpdateParameter? updateParameter = updateParameters.Find(x => x.ColumnName == column);
                    if (updateParameter != null)
                    {
                        columnValues.Add($"`{column}` = {updateParameter.Value!.ParameterName}");
                        command.Parameters.Add(updateParameter.Value!);
                    }
                    
                    QueryParameter? queryParameter = queryParameters.FirstOrDefault( x=> x.ColumnName == column);
                    if (queryParameter != null)
                    {
                        string paramName = $"@q{qParamIndex++}";
                        queryList.Add($"`{column}` {queryParameter.Operator} {paramName}");
                        command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(queryParameter.Value));
                    }
                }
                
                string values =  string.Join(", ", columnValues);
                string queryValues = string.Join(" AND ", queryList);
                command.CommandText = $"UPDATE `{_tableName}` SET {values} WHERE {queryValues};";
                
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(UpdateAsync), ex);
                return  false;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Deletes an entity from the database by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to delete.</param>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <returns><see langword="true"/> if the deletion was successful; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DeleteAsync(ID id, MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return false;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return false;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                command.Parameters.AddWithValue("@Id", id);
                command.CommandText = $"DELETE FROM `{_tableName}` WHERE `{_idColumnName}` = @Id;";
                
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(DeleteAsync), ex);
                return false;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }
        
        /// <summary>
        /// Deletes entities matching the specified query parameters.
        /// </summary>
        /// <param name="queryParameters">The conditions to match for the deletion.</param>
        /// <returns><see langword="true"/> if at least one row was deleted; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DeleteAsync(params QueryParameter[] queryParameters) =>
            await DeleteAsync(null, null, queryParameters);

        /// <summary>
        /// Deletes entities matching the specified query parameters.
        /// </summary>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <param name="queryParameters">The conditions to match for the deletion.</param>
        /// <returns><see langword="true"/> if at least one row was deleted; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DeleteAsync(MySqlConnection? connection, MySqlTransaction? transaction, params QueryParameter[] queryParameters)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null || queryParameters.Length == 0)
                return false;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return false;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                List<string> queryList = new  List<string>();
                int qParamIndex = 0;
                foreach (var column in _columnMappings.Values)
                {
                    if (column == _idColumnName)
                        continue;
                    
                    QueryParameter? queryParameter = queryParameters.FirstOrDefault( x=> x.ColumnName == column);
                    if (queryParameter != null)
                    {
                        string paramName = $"@q{qParamIndex++}";
                        queryList.Add($"`{column}` {queryParameter.Operator} {paramName}");
                        command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(queryParameter.Value));
                    }
                }
                
                string queryValues = string.Join(" AND ", queryList);
                command.CommandText = $"DELETE FROM `{_tableName}` WHERE {queryValues};";
                
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(DeleteAsync), ex);
                return false;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }
        
        /// <summary>
        /// Deletes multiple rows from the database table where the specified column matches any of the given values.
        /// </summary>
        /// <param name="columnName">The column name to filter by.</param>
        /// <param name="values">The list of values to match against the column.</param>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <returns><see langword="true"/> if at least one row was deleted; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DeleteRangeAsync(string columnName, List<object> values, MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return false;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return false;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                var paramNames = new List<string>();
                for (int i = 0; i < values.Count; i++)
                {
                    string paramName = $"@p{i}";
                    command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(values[i]));
                    paramNames.Add(paramName);
                }
                
                string inClause = string.Join(", ", paramNames);
                command.CommandText = $"DELETE FROM `{_tableName}` WHERE `{columnName}` IN ({inClause});";
                
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                HandleException(nameof(DeleteAsync), ex);
                return false;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Retrieves an entity from the database by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to retrieve.</param>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <returns>The matching entity, or <see langword="null"/> if not found or the operation failed.</returns>
        public async Task<T?> GetAsync(ID id, MySqlConnection? connection = null, MySqlTransaction? transaction = null)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null)
                return null;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return null;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                command.Parameters.AddWithValue("@Id", id);
                command.CommandText = $"SELECT * FROM `{_tableName}` WHERE `{_idColumnName}` = @Id;";

                await using var reader = await command.ExecuteReaderAsync();
                T? result = null;
                if (await reader.ReadAsync())
                    result = reader.ConvertToObject<T>();
                return result;
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAsync), ex);
                return null;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }

        /// <summary>
        /// Retrieves entities matching the specified query parameters.
        /// </summary>
        /// <param name="limit">The maximum number of rows to return.</param>
        /// <param name="queryParameters">The conditions to match for the query.</param>
        /// <returns>A list of matching entities, or <see langword="null"/> if none found or the operation failed.</returns>
        public async Task<List<T>?> GetAsync(int limit = 1000, params QueryParameter[] queryParameters) =>
            await GetAsync(null, null, limit, queryParameters);


        /// <summary>
        /// Retrieves entities matching the specified query parameters.
        /// </summary>
        /// <param name="connection">An optional existing database connection to reuse.</param>
        /// <param name="transaction">An optional transaction to execute within.</param>
        /// <param name="limit">The maximum number of rows to return.</param>
        /// <param name="queryParameters">The conditions to match for the query.</param>
        /// <returns>A list of matching entities, or <see langword="null"/> if none found or the operation failed.</returns>
        public async Task<List<T>?> GetAsync(MySqlConnection? connection, MySqlTransaction? transaction, int limit = 1000, params QueryParameter[] queryParameters)
        {
            bool isLocalConnection = connection == null;
            connection ??= _databaseManager.CreateConnection();
            if (connection == null || queryParameters.Length == 0)
                return null;
            bool connectionState = await connection.OpenSafeAsync();
            if (!connectionState)
                return null;
            
            try
            {
                await using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                List<string> queryList = new  List<string>();
                int qParamIndex = 0;
                foreach (var column in _columnMappings.Values)
                {
                    if (column == _idColumnName)
                        continue;
                    
                    QueryParameter? queryParameter = queryParameters.FirstOrDefault( x=> x.ColumnName == column);
                    if (queryParameter != null)
                    {
                        string paramName = $"@q{qParamIndex++}";
                        queryList.Add($"`{column}` {queryParameter.Operator} {paramName}");
                        command.Parameters.AddWithValue(paramName, SqlTypeHelper.FixValue(queryParameter.Value));
                    }
                }
                
                string queryValues = string.Join(" AND ", queryList);
                
                command.CommandText = $"SELECT * FROM `{_tableName}` WHERE {queryValues} LIMIT {limit};";

                await using var reader = await command.ExecuteReaderAsync();
                List<T>? result = null;
                while (await reader.ReadAsync())
                {
                    result ??= new List<T>();
                    var obj = reader.ConvertToObject<T>();
                    if (obj == null) continue;
                    result.Add(obj);
                }
                return result;
            }
            catch (Exception ex)
            {
                HandleException(nameof(GetAsync), ex);
                return null;
            }
            finally
            {
                if (isLocalConnection) await connection.CloseAsync();
            }
        }
        
        private static void HandleException(string methodName, Exception ex)
        {
            if (ex is OperationCanceledException canceledEx)
                throw canceledEx;
            
            LoggerHelper.LogError($"Error in TLibrary while exectuin {methodName}:");
            LoggerHelper.LogError(ex);
        }
    }
}