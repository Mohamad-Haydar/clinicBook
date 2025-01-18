using System.Security.Claims;
using api.BusinessLogic.DataAccess;
using System.Text.Json;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Helper;

namespace api.Controllers;

[Authorize]
public class TokenController : Controller
{
    private readonly ITokenData _tokenData;
    private readonly IMessageProvider _messageProvider;
    public TokenController(ITokenData tokenData, IMessageProvider messageProvider)
    {
        _tokenData = tokenData;
        _messageProvider = messageProvider;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh()
    {
        RefreshRequest tokenApiModel = new()
        {
            AccessToken = Request.Cookies["accessToken"],
            RefreshToken = Request.Cookies["refreshToken"]
        };

        if (tokenApiModel.AccessToken == null || tokenApiModel.RefreshToken == null)
            return BadRequest("Invalid client request");

        try
        {
            var result = await _tokenData.RefreshAsync(tokenApiModel).ConfigureAwait(false);
            if(!result.IsSuccess) return BadRequest(result);
            var userDataJson = JsonSerializer.Serialize(new
            {
                id = result.Data.Id,
                firstName = result.Data.FirstName,
                lastName = result.Data.LastName,
                email = result.Data.Email,
                phoneNumber = result.Data.PhoneNumber,
                roles = result.Data.Roles,
            });

            Response.Cookies.Append("userData", userDataJson, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });
            Response.Cookies.Append("accessToken", result.Data.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });

            Response.Cookies.Append("refreshToken", result.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });
            return Ok(new
            {
                Id = result.Data.Id,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName,
                Email = result.Data.Email,
                PhoneNumber = result.Data.PhoneNumber,
            });
        }
        catch (InvalidRequestException)
        {
            Response.Cookies.Delete("accessToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
            Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
            return BadRequest(Result.Failure(_messageProvider.GetMessage("pleaseLoginError")));
        }
        catch (Exception ex)
        {
           return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
        
    }

}

        