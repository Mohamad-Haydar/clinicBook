using api.BusinessLogic.DataAccess;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace api.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["accessToken"];
            var refreshToken = context.Request.Cookies["refreshToken"];
            var userData = context.Request.Cookies["userData"];
            if (accessToken != null) 
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _tokenData = scope.ServiceProvider.GetRequiredService<ITokenData>();
                    var token = accessToken.ToString().Trim();

                    if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(userData))
                    {
                        var jwtHandler = new JwtSecurityTokenHandler();
                        if (jwtHandler.CanReadToken(token))
                        {
                            var jwtToken = jwtHandler.ReadJwtToken(token);
                            var expirationDate = jwtToken.ValidTo;
                            var result = new AuthenticationResponse()
                            {
                                AccessToken = accessToken,
                                RefreshToken = refreshToken,
                            };
                            if (expirationDate < DateTime.UtcNow)
                            {
                                result = await _tokenData.RefreshAsync(new RefreshRequest { AccessToken = accessToken, RefreshToken = refreshToken });
                                context.Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Lax,
                                    Expires = DateTime.UtcNow.AddYears(1)
                                });

                                context.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Lax,
                                    Expires = DateTime.UtcNow.AddYears(1)
                                });
                            }
                            context.Request.Headers.Authorization = $"bearer {result.AccessToken}";
                        }
                    }

                }
            }

            await _next(context);
        }
    }
}
