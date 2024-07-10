using api.library.Internal.DataAccess;
using api.library.Models;
using Microsoft.Extensions.Configuration;

namespace api.library.DataAccess;

public class SecretaryData
{
    private readonly IConfiguration _configuration;

    public SecretaryData(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<SecretaryModel>> GetSecretaries()
    {
        SqlDataAccess sql = new(_configuration);
        var output = await sql.LoadDataAsync<SecretaryModel, dynamic>("f_load_secretaries", new { }, "appData");
        return output;
    }
}