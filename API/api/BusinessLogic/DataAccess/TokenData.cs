using System.Security.Claims;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;

namespace api.BusinessLogic.DataAccess;

public class TokenData
{
    private readonly UserManager<UserModel> _userManager;
    private readonly IdentityAppDbContext _identityContext;
    private readonly ITokenService _tokenService;
    public TokenData(UserManager<UserModel> userManager, IdentityAppDbContext identityContext, ITokenService tokenService)
    {
        _userManager = userManager;
        _identityContext = identityContext;
        _tokenService = tokenService;
    }
    public async Task<AuthenticationResponse> RefreshAsync(RefreshRequest tokenApiModel)
    {
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
                 throw new InvalidRequestException("Invalid client request");
            }
            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(email);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _identityContext.SaveChanges();

            return new AuthenticationResponse()
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken
                    };
        }
        catch (Exception)
        {
            throw new BusinessException("Something went wrong. Please try again.");
        }
        
    }
}