using System.Security.Claims;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.BusinessLogic.DataAccess;

public class TokenData : ITokenData
{
    private readonly UserManager<UserModel> _userManager;
    private readonly IdentityAppDbContext _identityContext;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _appContext;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public TokenData(UserManager<UserModel> userManager, IdentityAppDbContext identityContext, ITokenService tokenService, ApplicationDbContext appContext)
    {
        _userManager = userManager;
        _identityContext = identityContext;
        _tokenService = tokenService;
        _appContext = appContext;
    }
    public async Task<AuthenticationResponse> RefreshAsync(RefreshRequest tokenApiModel)
    {
        //await _semaphore.WaitAsync();
        string? accessToken = tokenApiModel.AccessToken;
        string? refreshToken = tokenApiModel.RefreshToken;
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var email = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            var roles = principal.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);
            var user = await _userManager.FindByIdAsync(userId);


            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new InvalidRequestException();
            }

            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(email);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _identityContext.SaveChangesAsync();

            return new AuthenticationResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Roles = roles
            };
        }
        catch (InvalidRequestException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new BusinessException();
        }
        finally
        {
            //_semaphore.Release();
        }

    }
}