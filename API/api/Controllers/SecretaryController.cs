using api.Attributes;
using api.Data;
using api.BusinessLogic.DataAccess;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

public class SecretaryController : Controller
{

    private readonly SecretaryData _secretaryData;
    private readonly ApplicationDbContext _appDbContext;

    public SecretaryController(SecretaryData secretaryData, ApplicationDbContext appDbContext)
    {
        _secretaryData = secretaryData;
        _appDbContext = appDbContext;
    }

    // [AuthorizeRoles(Roles.Admin)]
    [Route("GetSecretaries")]
    [HttpGet]
    public async Task<IActionResult> GetSecretaries(string email)
    {
        try
        {
            var secretarie = await _appDbContext.Secretaries.FirstOrDefaultAsync(x => x.Email == email);
            return Ok(secretarie);
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "something went wrong please try again"});
        }
    }
}