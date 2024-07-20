using api.Helper;
using api.Internal.DataAccess;
using api.Models.Responce;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace api.BusinessLogic.DataAccess;

public class SecretaryData
{
    private readonly IOptions<ConnectionStrings> _connectionStrings;

    public SecretaryData(IOptions<ConnectionStrings> connectionStrings)
    {
        _connectionStrings = connectionStrings;
    }
}