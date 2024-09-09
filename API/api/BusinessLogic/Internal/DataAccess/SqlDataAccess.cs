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
    public async Task<List<Dictionary<string, object>>> LoadDataAsync(string functionName, string[] paramNames, object[] paramValues, string connectionString)
    {
        try
        {
            var results = new List<Dictionary<string, object>>();
            await using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                await using (var cmd = new NpgsqlCommand())
                {     
                    cmd.Connection = connection;
                    string sql = $"SELECT * FROM {functionName}({string.Join(", ", Array.ConvertAll(paramNames, name => $"@{name}"))})";
                    cmd.CommandText = sql;
                    
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        cmd.Parameters.AddWithValue(paramNames[i], paramValues[i]);
                    }
                    await cmd.PrepareAsync().ConfigureAwait(false);

                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            object value = reader[i];
                            row[reader.GetName(i)] = value;
                        }
                        results.Add(row);
                    }
                }
            }
            return results;
         }
        catch (NpgsqlException ex)
        {
            if (ex.InnerException?.Message.StartsWith("M3GA0:") == true)
            {
                throw new BusinessException(ex.InnerException.Message[8..]);
            }
            throw new BusinessException();
        }

        catch (Exception )
        {
          throw new BusinessException();
        }
    }

    public async Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionString)
    {
        try
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.StartsWith("M3GA0:") == true)
            {
                throw new BusinessException(ex.Message[8..]);
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
        IEnumerable<T> rows = await _connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ConfigureAwait(false);
        return rows;
    }
    public async Task SaveDataInTransactionAsync<T>(string storedProcedure, T parameters)
    {
        await _connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ConfigureAwait(false);
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