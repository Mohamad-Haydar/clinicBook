using api.Attributes;
using api.Data;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;
using api.Models.Responce;
using api.Helper;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin)]
public class SecretaryController : Controller
{

    private readonly ISecretaryData _secretaryData;
    private readonly IMessageProvider _messageProvider;

    public SecretaryController(ISecretaryData secretaryData, IMessageProvider messageProvider)
    {
        _secretaryData = secretaryData;
        _messageProvider = messageProvider;
    }

    [Route("GetSecretaries")]
    [HttpGet]
    public async Task<IActionResult> GetSecretarieByEmail(string email)
    {
        try
        {
            var res = await _secretaryData.GetSecretariebyEmailAsync(email).ConfigureAwait(false);
            return Ok(res.Data);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }
}