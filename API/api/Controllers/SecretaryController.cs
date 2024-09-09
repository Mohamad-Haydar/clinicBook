using api.Attributes;
using api.Data;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;
using api.Models.Responce;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin)]
public class SecretaryController : Controller
{

    private readonly ISecretaryData _secretaryData;

    public SecretaryController(ISecretaryData secretaryData)
    {
        _secretaryData = secretaryData;
    }

    [Route("GetSecretaries")]
    [HttpGet]
    public async Task<IActionResult> GetSecretarieByEmail(string email)
    {
        try
        {
            var secretarie = await _secretaryData.GetSecretariebyEmailAsync(email).ConfigureAwait(false);
            return Ok(secretarie);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }
}