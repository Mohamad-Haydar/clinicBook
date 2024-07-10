using System.Security.Claims;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(string email);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}