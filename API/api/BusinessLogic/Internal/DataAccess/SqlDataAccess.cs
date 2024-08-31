using System.Data;
using api.BusinessLogic.DataAccess;
using api.Exceptions;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace api.Internal.DataAccess;

public class SqlDataAccess :ISqlDataAccess,  IDisposable
{
    public async Task<IQueryable<Dictionary<string, object>>> LoadDataAsync(string functionName, string[] paramNames, object[] paramValues, string connectionString)
    {
        try
        {
            var results = new List<Dictionary<string, object>>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand())
                {     
                    cmd.Connection = connection;
                    string sql = $"SELECT * FROM {functionName}({string.Join(", ", Array.ConvertAll(paramNames, name => $"@{name}"))})";
                    cmd.CommandText = sql;
                    
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        cmd.Parameters.AddWithValue(paramNames[i], paramValues[i]);
                    }

                    var reader = cmd.ExecuteReaderAsync().Result;
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader[i];
                            row[columnName] = value;
                        }
                        results.Add(row);
                    }
                }
                await connection.CloseAsync();
            }
            return results.AsQueryable();
         }
        
        catch (Exception ex)
        {
            if(ex.InnerException?.Message.StartsWith("P0001:") == true)
            {
                throw new BusinessException(ex.InnerException.Message[6..]);
            }
          //throw new Exception(ex.Message[37..]);
          throw new BusinessException();
        }
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionString)
    {
        try
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.StartsWith("P0001:") == true)
            {
                throw new BusinessException(ex.Message[6..]);
            }
            //throw new Exception(ex.Message[37..]);
            throw new BusinessException();
        }
    }

    #region Load and Save in Transaction

    private IDbConnection _connection;
    private IDbTransaction _transaction;
    public void StartTransaction(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }
    public async Task<IEnumerable<T>> LoadDataInTransactionAsync<T, U>(string storedProcedure, U parameters)
    {
        IEnumerable<T> rows = await _connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        return rows;
    }
    public async Task SaveDataInTransactionAsync<T>(string storedProcedure, T parameters)
    {
        await _connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
    }
    private bool isCloded = false;

    public IConfiguration Config { get; }

    public void CommitTransaction()
    {
        _transaction?.Commit();
        _connection?.Close();
        isCloded = true;
    }
    public void RollbackTransaction()
    {
        _transaction?.Rollback();
        _connection?.Close();
        isCloded = true;
    }

    public void Dispose()
    {
        if (!isCloded)
        {
            try
            {
                CommitTransaction();
            }
            catch
            {
                // TODO: Log this issue
            }
        }
        _transaction = null;
        _connection = null;
    }
    #endregion
}