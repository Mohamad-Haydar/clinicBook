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

namespace api.Controllers;

[Authorize]
public class TokenController : Controller
{
    private readonly ITokenData _tokenData;
    public TokenController(ITokenData tokenData)
    {
        _tokenData = tokenData;
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
            var result = await _tokenData.RefreshAsync(tokenApiModel);
            
            var userDataJson = JsonSerializer.Serialize(new
            {
                id = result.Id,
                userName = result.UserName,
                email = result.Email,
                phoneNumber = result.PhoneNumber,
                roles = result.Roles,
            });

            Response.Cookies.Append("userData", userDataJson, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });
            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });
            return Ok(new
            {
                Id = result.Id,
                UserName = result.UserName,
                Email = result.Email,
                PhoneNumber = result.PhoneNumber,
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
            return BadRequest(new {message = "please login in"});
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        
    }

}

        