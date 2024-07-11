using api.library.Helper;
using api.library.Internal.DataAccess;
using api.library.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace api.library.DataAccess;

public class SecretaryData
{
    // private readonly IConfiguration _configuration;
    private IOptions<ConnectionStrings> _connectionStrings;
    private readonly ISqlDataAccess _sql;

    public SecretaryData(ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings)
    {
        _sql = sql;
        _connectionStrings = connectionStrings;
    }

    public async Task<IEnumerable<SecretaryModel>> GetSecretaries()
    {
        var output = await _sql.LoadDataAsync<SecretaryModel, dynamic>("f_load_secretaries", new { }, _connectionStrings.Value.AppDbConnection);
        return output;
    }
}