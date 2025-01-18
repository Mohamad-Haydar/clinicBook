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
                 using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var accessToken = context.Request.Cookies["accessToken"];
                    var refreshToken = context.Request.Cookies["refreshToken"];
                    var userData = context.Request.Cookies["userData"];
                    if (accessToken != null)
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
                                    if (expirationDate > DateTime.UtcNow)
                                    {
                                        context.Request.Headers.Authorization = $"bearer {result.AccessToken}";
                                    }
                                    else
                                    { 
                                        var res = await _tokenData.RefreshAsync(new RefreshRequest { AccessToken = accessToken, RefreshToken = refreshToken });
                                        context.Response.Cookies.Append("accessToken", res.Data.AccessToken, new CookieOptions
                                        {
                                            HttpOnly = true,
                                            Secure = true,
                                            SameSite = SameSiteMode.Lax,
                                            Expires = DateTime.UtcNow.AddYears(1)
                                        });

                                        context.Response.Cookies.Append("refreshToken", res.Data.RefreshToken, new CookieOptions
                                        {
                                            HttpOnly = true,
                                            Secure = true,
                                            SameSite = SameSiteMode.Lax,
                                            Expires = DateTime.UtcNow.AddYears(1)
                                        });
                                        context.Request.Headers.Authorization = $"bearer {res.Data.AccessToken}";
                                    }
                                
                                }
                            }
                            catch (Exception)
                            {
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
                                return;
                            }
                        }
                    }
                 }
            }
            await _next(context);
        }
    
    }
}
