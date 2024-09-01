using api.BusinessLogic.DataAccess;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Exceptions;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace api.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static MemoryCache _tokenCache = new(new MemoryCacheOptions());


        public TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {           
            if (!context.Request.Path.Value.EndsWith("logout"))
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
                            try
                            {
                                var jwtHandler = new JwtSecurityTokenHandler();
                                if (jwtHandler.CanReadToken(token))
                                {
                                    var jwtToken = jwtHandler.ReadJwtToken(token);
                                    var expirationDate = jwtToken.ValidTo;
                                    var cacheKey = $"{refreshToken}";
                                    var result = new AuthenticationResponse()
                                    {
                                        AccessToken = accessToken,
                                        RefreshToken = refreshToken,
                                    };
                                    await _semaphore.WaitAsync();
                                    if (_tokenCache.TryGetValue(cacheKey, out AuthenticationResponse cachedToken))
                                    {
                                        if (cachedToken != null && expirationDate < DateTime.UtcNow)
                                        {
                                            // Use cached token if it exists and is valid
                                            context.Request.Headers.Authorization = $"bearer {cachedToken.AccessToken}";
                                            _semaphore.Release();
                                            await _next(context);
                                        }
                                    }
                                    else if (!_tokenCache.TryGetValue(cacheKey, out cachedToken) || cachedToken == null || expirationDate < DateTime.UtcNow)
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
                                        context.Request.Headers.Authorization = $"bearer {result.AccessToken}";
                                    }
                                    _tokenCache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
                                    _semaphore.Release();
                                }
                            }
                            catch (Exception)
                            {
                                context.Response.Cookies.Delete("userData", new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Lax
                                });
                                context.Response.Cookies.Delete("accessToken", new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Lax
                                });
                                context.Response.Cookies.Delete("refreshToken", new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Lax
                                });
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = 400;
                                var responseResult = new
                                {
                                    message = "حدث خطأ, الرجاء اعادة تسجيل الدخول.",
                                    isSuccess = false
                                };

                                // Serialize the object to a JSON string
                                string jsonResponse = JsonSerializer.Serialize(responseResult);

                                // Convert the JSON string to a byte array (buffer)
                                byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

                                // Write the buffer to the response body
                                await context.Response.Body.WriteAsync(buffer);
                            }
                        }
                    }
                }
            }
            await _next(context);
        }
    }
}
