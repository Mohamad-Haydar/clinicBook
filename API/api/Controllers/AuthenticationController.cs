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
using Serilog;
using System.Text;
using System.Security.Cryptography;

namespace api.Controllers;

[AllowAnonymous]
public class AuthenticationController : Controller
{
    private readonly IAuthenticationData _authenticationData;
    private readonly UserManager<UserModel> _userManager;

    public AuthenticationController(IAuthenticationData authenticationData, UserManager<UserModel> userManager)
    {
        _authenticationData = authenticationData;
        _userManager = userManager;
    }

    [Route("GenerateClients")]
    [HttpPost]
    //[AuthorizeRoles(Roles.Admin)]
    [AllowAnonymous]
    public async Task GenerateClients()
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

        //StringBuilder name = new();
        //StringBuilder email = new();
        //string emailtremination = "@gmail.com";
        //var rng = RandomNumberGenerator.Create();

        //var randomNumber = new byte[12];

        //for (int i = 0; i < 3000; i++)
        //{ 
        //    name.Clear();
        //    email.Clear();
        //    rng.GetBytes(randomNumber);
        //    name.Append(Convert.ToBase64String(randomNumber));
        //    email.Append(name);
        //    email.Append(emailtremination);

        //    await _authenticationData.RegisterClientAsync(new()
        //    {
        //        FirstName = name.ToString(),
        //        LastName = name.ToString(),
        //        Email = email.ToString(),
        //        Password = "Pass1234!",
        //        ConfirmPassword = "Pass1234!",
        //        PhoneNumber = "76121212"
        //    });
        //}
    }

    [Route("RegisterClient")]
    [HttpPost]
    public async Task<IActionResult> RegisterClient([FromBody] CreateUserRequest model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var result = await _authenticationData.RegisterClientAsync(model).ConfigureAwait(false);
            var userDataJson = JsonSerializer.Serialize(new CookieUserModel
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
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });

            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
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
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }

    }

    [Route("RegisterSecretary")]
    [HttpPost]
    [AuthorizeRoles(Roles.Admin, Roles.Secretary)]
    public async Task<IActionResult> RegisterSecretary([FromBody] CreateSecretaryRequest model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _authenticationData.RegisterSecretaryAsync(model).ConfigureAwait(false);
            return Ok(new Response("لقد تم انشاء حساب سكرتيرة جديد بنجاح."));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [Route("RegisterDoctor")]
    [HttpPost]
    [AuthorizeRoles(Roles.Secretary, Roles.Admin)]
    public async Task<IActionResult> RegisterDoctor([FromBody] CreateDoctorRequest model)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _authenticationData.RegisterDoctorAsync(model).ConfigureAwait(false);
          
            return Ok(new Response("لقد تم انشاء حساب دكتور جديد بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response (ex.Message));
        }
    }

    [Route("RegisterAdmin")]
    [HttpPost]
    [AuthorizeRoles(Roles.Admin)]
    public async Task<IActionResult> RegisterAdmin(string email, string password)
    {
        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new Response ("Please enter a valid input"));
        }
        try
        {
            await _authenticationData.RegisterAdminAsync(email, password).ConfigureAwait(false);
            return Ok(new { message = "Admin created successfully. You can login to your account." });
        }
        catch (UserAlreadyExistsException ex)
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
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var result = await _authenticationData.LoginUserAsync(model).ConfigureAwait(false);

            var userDataJson = JsonSerializer.Serialize(new CookieUserModel
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
                SameSite = SameSiteMode.Lax,
                Expires  = DateTime.UtcNow.AddYears(1)
            });

            Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddYears(1)
            });

            return Ok(new{
                Id = result.Id,
                UserName = result.UserName,
                Email = result.Email,
                PhoneNumber = result.PhoneNumber,
                Roles = result.Roles,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }   
    }

    [Route("logout")]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        //string? refreshToken = Request.Cookies["refreshToken"];
        //string? accessToken = Request.Cookies["accessToken"] ;
        //string? userData = Request.Cookies["userData"] ;

        //try
        //{
        //if (refreshToken == null || accessToken == null || userData == null)
        //{
        //    throw new BusinessException();
        //}
        //await _authenticationData.LogoutAsync(refreshToken, accessToken).ConfigureAwait(false);
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
            return Ok(new Response("لقد تم تسجيل خروجك بنجاح"));
        //}
        //catch (Exception ex)
        //{
        //    Response.Cookies.Delete("userData", new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Lax
        //    });
        //    Response.Cookies.Delete("accessToken", new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Lax
        //    });
        //    Response.Cookies.Delete("refreshToken", new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.Lax
        //    });
        //    return BadRequest(new Response(ex.Message));
        //}
    }

}