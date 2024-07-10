using api.library.DataAccess;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class SecretaryController : Controller
{

    private readonly SecretaryData _secretaryData;

    public SecretaryController(SecretaryData secretaryData)
    {
        _secretaryData = secretaryData;
    }

    [AuthorizeRoles(Roles.Secretary)]
    [Route("GetSecretaries")]
    [HttpGet]
    public async Task<IActionResult> GetSecretaries()
    {
        try
        {
            var secretaries = await _secretaryData.GetSecretaries();
            return Ok(secretaries);
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "something went wrong please try again"});
        }
    }
}