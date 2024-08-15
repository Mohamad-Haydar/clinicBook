using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using api.Attributes;
using api.Data;
using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.BusinessLogic.DataAccess;
using System.Text.Json;

namespace api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IAuthenticationData _authenticationData;

    public AuthenticationController(IAuthenticationData authenticationData)
    {
        _authenticationData = authenticationData;
    }

    [Route("GenerateInitialData")]
    [HttpPost]
    public async Task GenerateInitialData()
    {
        // var values = Enum.GetValues(typeof(Roles));
        // foreach (var val in values)
        // {
        //     var role = val.ToString();
        //     bool roleExists = await _roleManager.RoleExistsAsync(role);
        //     if (roleExists == false)
        //     {
        //         await _roleManager.CreateAsync(new IdentityRole(role));
        //     }
        // }

        // string[] emails = ["zeinab@gmail.com", "mohamad@gmail.com"];
        // foreach (var email in emails)
        // {
        //     var userExists = await _userManager.FindByEmailAsync(email);
        //     if(userExists == null)
        //     {
        //         var user = new IdentityUser{UserName=email, Email=email};
        //         var result = await _userManager.CreateAsync(user, "Pass1234!");
        //         if(result.Succeeded)
        //         {
        //             await _userManager.AddToRoleAsync(user, "Secretary");
        //         } 
        //     }
        // }
    }

    [Route("RegisterClient")]
    [HttpPost]
    public async Task<IActionResult> RegisterClient([FromBody] CreateUserRequest model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response("Please enter a valid input"));
        }
        try
        {
            var result = await _authenticationData.RegisterClientAsync(model);
            var userDataJson = JsonSerializer.Serialize(new
            {
                id= result.Id,
                userName= result.UserName,
                email= result.Email,
                phoneNumber= result.PhoneNumber,
                roles = result.Roles,
            });

            Response.Cookies.Append("userData", userDataJson, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            return Ok(new
            {
                Id = result.Id,
                UserName = result.UserName,
                Email = result.Email,
                PhoneNumber = result.PhoneNumber,
                Roles = result.Roles,
            });
        }
        catch (UserExistsException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (WrongPasswordException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something went wrong. Please try again." ));
        }

    }

    [Route("RegisterSecretary")]
    [HttpPost]
    public async Task<IActionResult> RegisterSecretary([FromBody] CreateSecretaryRequest model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response("Please enter a valid input"));
        }
        try
        {
            await _authenticationData.RegisterSecretaryAsync(model);
            /* var userDataJson = JsonSerializer.Serialize(new
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
                 SameSite = SameSiteMode.Lax
             });

             Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
             {
                 HttpOnly = true,
                 Secure = true,
                 SameSite = SameSiteMode.Lax
             });

             Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
             {
                 HttpOnly = true,
                 Secure = true,
                 SameSite = SameSiteMode.Lax
             });
            */

            return Ok(new Response("لقد تم انشاء حساب سكرتيرة جديد بنجاح."));
        }
        catch (UserExistsException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something went wrong. Please try again."));
        }
    }

    [AuthorizeRoles(Roles.Secretary, Roles.Admin)]
    [Route("RegisterDoctor")]
    [HttpPost]
    public async Task<IActionResult> RegisterDoctor([FromBody] CreateDoctorRequest model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response("Please enter a valid input"));
        }
        try
        {
            await _authenticationData.RegisterDoctorAsync(model);
           /* var userDataJson = JsonSerializer.Serialize(new
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
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
           */

            return Ok(new Response("لقد تم انشاء حساب دكتور جديد بنجاح"));
        }
        catch (UserExistsException ex)
        {
            return BadRequest(new Response (ex.Message ));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response (ex.Message ));
        }
        catch (Exception)
        {
            return BadRequest(new Response ("Something went wrong. Please try again." ));
        }
    }

    [Route("RegisterAdmin")]
    [HttpPost]
    public async Task<IActionResult> RegisterAdmin(string email, string password)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response ("Please enter a valid input"));
        }
        try
        {
            await _authenticationData.RegisterAdminAsync(email, password);
            return Ok(new { message = "Admin created successfully. You can login to your account." });
        }
        catch (UserExistsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Something went wrong. Please try again." });
        }
    }


    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> LoginUser([Required] LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response("Please enter a valid input"));
        }
        try
        {
            var result = await _authenticationData.LoginUserAsync(model);

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
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });


            return Ok(new{
                Id = result.Id,
                UserName = result.UserName,
                Email = result.Email,
                PhoneNumber = result.PhoneNumber,
                Roles = result.Roles,
            });
        }
        catch (UserExistsException ex)
        {
            return BadRequest(new Response(ex.Message ));
        }
         catch (WrongPasswordException ex)
        {
            return BadRequest(new Response(ex.Message ));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message ));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something went wrong. Please try again." ));
        }   
    }

    [Route("logout")]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        string? accessToken = Request.Cookies["accessToken"] ;
        string? userData = Request.Cookies["userData"] ;

        try
        {
            if (refreshToken == null || accessToken == null || userData == null)
            {
                throw new Exception();
            }
            await _authenticationData.LogoutAsync(refreshToken, accessToken);
            Response.Cookies.Delete("userData", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
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
            return Ok(new Response("Log out Successfully"));
        }
        catch (Exception)
        {
            Response.Cookies.Delete("userData", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
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
            return BadRequest(new Response("Invalid client request"));
        }
    }

}