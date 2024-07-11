using api.library.DataAccess;
using api.library.Helper;
using api.library.Internal.DataAccess;
using api.library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ISqlDataAccess _sql;
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    public WeatherForecastController(ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings)
    {
        _sql = sql;
        _connectionStrings = connectionStrings;
    }

    [HttpGet]
    [Route("GetAllSecretaries")]
    public async Task<IEnumerable<SecretaryModel>> GetSecretaries()
    {
        SecretaryData secretaryData = new(_sql, _connectionStrings);

        return await secretaryData.GetSecretaries();

    }
}
