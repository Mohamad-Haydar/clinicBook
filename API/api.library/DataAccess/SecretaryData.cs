using api.library.Helper;
using api.library.Internal.DataAccess;
using api.library.Models.Responce;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace api.library.DataAccess;

public class SecretaryData
{
    private readonly IOptions<ConnectionStrings> _connectionStrings;

    public SecretaryData(IOptions<ConnectionStrings> connectionStrings)
    {
        _connectionStrings = connectionStrings;
    }
}