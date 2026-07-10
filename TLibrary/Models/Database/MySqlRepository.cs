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
    public class MySqlRepository<ID, T> where T : class
    {
        protected readonly string _tableName;
        protected readonly string _idColumnName;
        protected readonly Type _classType;
        protected readonly IDatabaseManager _databaseManager;
        protected readonly Dictionary<PropertyInfo, string> _columnMappings = new Dictionary<PropertyInfo, string>();
        
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
                using var command = new MySqlCommand();
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
                using var command = new MySqlCommand();
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
                    command.Parameters.AddWithValue(paramName, value ?? DBNull.Value);
                }
                
                string columns =  string.Join(", ", columnList);
                string values =  string.Join(", ", columnValues);
                command.CommandText = $"INSERT INTO `{_tableName}` ({columns}) VALUES ({values});" +
                                      $"SELECT * FROM `{_tableName}` WHERE id = LAST_INSERT_ID();";

                T? result = null;
                using var reader = await command.ExecuteReaderAsync();
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
                using var command = new MySqlCommand();
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
                    command.Parameters.AddWithValue(paramName, value ?? DBNull.Value);
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

        public async Task<bool> UpdateAsync(List<UpdateParameter> updateParameters, params QueryParameter[] queryParameters) =>
            await UpdateAsync( null, null, updateParameters, queryParameters);
        
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
                using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                List<string> columnValues = new  List<string>();
                List<string> queryList = new  List<string>();
                int vParamIndex = 0;
                int qParamIndex = 0;
                foreach (var column in _columnMappings.Values)
                {
                    if (column == _idColumnName)
                        continue;
                    
                    UpdateParameter? updateParameter = updateParameters.Find(x => x.ColumnName == column);
                    if (updateParameter != null)
                    {
                        string paramName = $"@v{vParamIndex++}";
                        columnValues.Add($"`{column}` = {paramName}");
                        command.Parameters.Add(updateParameter.Value!);
                    }
                    
                    QueryParameter? queryParameter = queryParameters.FirstOrDefault( x=> x.ColumnName == column);
                    if (queryParameter != null)
                    {
                        string paramName = $"@q{qParamIndex++}";
                        queryList.Add($"`{column}` {queryParameter.Operator} {paramName}");
                        command.Parameters.AddWithValue(paramName, queryParameter.Value ?? DBNull.Value);
                    }
                }
                
                string values =  string.Join(", ", columnValues);
                string queryValues = string.Join(", ", queryList);
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
                using var command = new MySqlCommand();
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
        
        public async Task<bool> DeleteAsync(params QueryParameter[] queryParameters) =>
            await DeleteAsync(null, null, queryParameters);
        
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
                using var command = new MySqlCommand();
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
                        command.Parameters.AddWithValue(paramName, queryParameter.Value ?? DBNull.Value);
                    }
                }
                
                string queryValues = string.Join(", ", queryList);
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
                using var command = new MySqlCommand();
                command.Connection = connection;
                if (transaction != null) command.Transaction = transaction;
                
                command.Parameters.AddWithValue("@Id", id);
                command.CommandText = $"SELECT * FROM `{_tableName}` WHERE `{_idColumnName}` = @Id;";
                
                using var reader = await command.ExecuteReaderAsync();
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

        public async Task<List<T>?> GetAsync(int limit = 1000, params QueryParameter[] queryParameters) =>
            await GetAsync(null, null, limit, queryParameters);
        
        
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
                using var command = new MySqlCommand();
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
                        command.Parameters.AddWithValue(paramName, queryParameter.Value ?? DBNull.Value);
                    }
                }
                
                string queryValues = string.Join(", ", queryList);
                
                command.CommandText = $"SELECT * FROM `{_tableName}` WHERE {queryValues} LIMIT {limit};";
                
                using var reader = await command.ExecuteReaderAsync();
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