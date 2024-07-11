using System.Security.Claims;
using api.Data;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class TokenController : Controller
{
    private readonly UserManager<UserModel> _userManager;
    private readonly IdentityAppDbContext _identityContext;
    private readonly ITokenService _tokenService;
    public TokenController(UserManager<UserModel> userManager, ITokenService tokenService, IdentityAppDbContext identityContext)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _identityContext = identityContext;
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest tokenApiModel)
    {
        if (!ModelState.IsValid || tokenApiModel.AccessToken == null || tokenApiModel.RefreshToken == null)
            return BadRequest("Invalid client request");

        string? accessToken = tokenApiModel.AccessToken;
        string? refreshToken = tokenApiModel.RefreshToken;
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var email = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
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
        
                return BadRequest("Invalid client request");
            }
            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(email);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _identityContext.SaveChanges();

            Response.Cookies.Append("jwtToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            return Ok(new AuthenticationResponse()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (System.Exception)
        {
            
            return BadRequest(new {message = "please login in"});
        }
        
    }

}