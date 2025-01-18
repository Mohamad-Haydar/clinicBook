using System.Security.Claims;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Helper;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace api.BusinessLogic.DataAccess;

public class TokenData : ITokenData
{
    private readonly ILogger<TokenData> _logger;
    private readonly UserManager<UserModel> _userManager;
    private readonly IdentityAppDbContext _identityContext;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _appContext;
    private readonly IMemoryCache _cache;
    private readonly IMessageProvider _messageProvider;

    public TokenData(UserManager<UserModel> userManager, IdentityAppDbContext identityContext, ITokenService tokenService, ApplicationDbContext appContext, ILogger<TokenData> logger, IMemoryCache cache, IMessageProvider messageProvider)
    {
        _userManager = userManager;
        _identityContext = identityContext;
        _tokenService = tokenService;
        _appContext = appContext;
        _logger = logger;
        _cache = cache;
        _messageProvider = messageProvider;
    }
    public async Task<Result<AuthenticationResponse>> RefreshAsync(RefreshRequest tokenApiModel)
    {
        string? accessToken = tokenApiModel.AccessToken;
        string? refreshToken = tokenApiModel.RefreshToken;
        string cacherefreshToken = $"refresh-token:{refreshToken}";
        string cacheaccessToken = $"access-token:{accessToken}";
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var email = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            var roles = principal.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            var userData = await _appContext.Clients.FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                if(
                    _cache.TryGetValue(cacherefreshToken, out string? cachedRefreshToken) &&
                    _cache.TryGetValue(cacheaccessToken, out string? cachedAccessToken)
                )
                {
                    return Result<AuthenticationResponse>.Success(new AuthenticationResponse
                    {
                        Id = user.Id,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        Email = email,
                        PhoneNumber = user.PhoneNumber,
                        AccessToken = cachedAccessToken,
                        RefreshToken = cachedRefreshToken,
                        Roles = roles
                    });
                }else{
                    return Result<AuthenticationResponse>.Failure(_messageProvider.GetMessage("wrongInput"));
                }
            }

            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(email);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _cache.Set(cacherefreshToken, newRefreshToken, TimeSpan.FromSeconds(10));
            _cache.Set(cacheaccessToken, newAccessToken, TimeSpan.FromSeconds(10));
            await _identityContext.SaveChangesAsync().ConfigureAwait(false);

            return Result<AuthenticationResponse>.Success(new AuthenticationResponse
            {
                Id = user.Id,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Email = email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }
}