namespace api.library.DataAccess;

public interface ISqlDataAccess
{
    Task<IEnumerable<T>> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName);
     Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName);
     void StartTransaction(string connectionString);
    Task<IEnumerable<T>> LoadDataInTransactionAsync<T, U>(string storedProcedure, U parameters);
    Task SaveDataInTransactionAsync<T>(string storedProcedure, T parameters);
    void CommitTransaction();
    void RollbackTransaction();
    new void Dispose();
}