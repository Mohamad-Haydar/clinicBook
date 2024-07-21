using System.Security.Claims;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class TokenController : Controller
{
    private readonly ITokenData _tokenData;
    public TokenController(ITokenData tokenData)
    {
        _tokenData = tokenData;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest tokenApiModel)
    {
        if (!ModelState.IsValid || tokenApiModel.AccessToken == null || tokenApiModel.RefreshToken == null)
            return BadRequest("Invalid client request");

        try
        {
            var result = await _tokenData.RefreshAsync(tokenApiModel);
            Response.Cookies.Append("jwtToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
            return Ok(result);
        }
        catch (InvalidRequestException)
        {
            Response.Cookies.Delete("accessToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });
            Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });
            return BadRequest(new {message = "please login in"});
        }
        catch (BusinessException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (Exception)
        {
            return BadRequest(new {message="Something when wrong please try again."});
        }
        
    }

}

        