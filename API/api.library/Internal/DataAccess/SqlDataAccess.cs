using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace api.library.Internal.DataAccess;

public class SqlDataAccess : IDisposable
{
    private readonly IConfiguration _configuration;

    public SqlDataAccess(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string? GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name);
    }

    public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
    {
        string? connectionString = GetConnectionString(connectionStringName);
        using (IDbConnection connection = new NpgsqlConnection(connectionString))
        {
            List<T> rows = connection.Query<T>("SELECT * FROM " + storedProcedure + "()", parameters).ToList();
            return rows;
        }
    }

    public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        string? connectionString = GetConnectionString(connectionStringName);
        using (IDbConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }

    #region Load and Save in Transaction

    private IDbConnection _connection;
    private IDbTransaction _transaction;
    public void StartTransaction(string connectionStringName)
    {
        string? connectionString = GetConnectionString(connectionStringName);
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }
    public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
    {
        List<T> rows = _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();
        return rows;
    }
    public void SaveDataInTransaction<T>(string storedProcedure, T parameters)
    {
        _connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
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