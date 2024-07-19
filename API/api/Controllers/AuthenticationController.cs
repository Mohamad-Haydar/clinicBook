using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using api.Attributes;
using api.Data;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IdentityAppDbContext _identityContext;
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _appContext;
    private readonly ITokenService _tokenService;

    public AuthenticationController(IdentityAppDbContext identityContext, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext appContext, ITokenService tokenService)
    {
        _identityContext = identityContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _appContext = appContext;
        _tokenService = tokenService;
    }

    [Route("GenerateInitialData")]
    [HttpPost]
    public async Task GenerateInitialData()
    {
        var values = Enum.GetValues(typeof(Roles));
        foreach (var val in values)
        {
            var role = val.ToString();
            bool roleExists = await _roleManager.RoleExistsAsync(role);
            if (roleExists == false)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

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
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        try
        {
            return Ok(new { message = "Client created successfully. You can login to your account." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Something went wrong. Please try again." });
        }

    }

    [Route("RegisterSecretary")]
    [HttpPost]
    public async Task<IActionResult> RegisterSecretary([FromBody] CreateSecretaryRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if(userExists != null)
        {
            return BadRequest(new {message="email already exists"});
        }

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                var user = new UserModel { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, Roles.Secretary.ToString());
                SecretaryModel secretary = new(){
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                await _appContext.Secretaries.AddAsync(secretary);
                await _appContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "Secretary created successfully. You can login to your account." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { message = ex.Message });
            }
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
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if(userExists != null)
        {
            return BadRequest(new {message="email already exists"});
        }

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                string userName =  model.FirstName +  model.LastName;
                var user = new UserModel
                {
                    UserName = userName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    RefreshToken = "",
                    RefreshTokenExpiryTime = DateTime.MinValue
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, Roles.Doctor.ToString());
                DoctorModel doctor = new(){
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Description = model.Description,
                    CategoryId = model.CategoryId
                };
                await _appContext.Doctors.AddAsync(doctor);
                await _appContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "Doctor created successfully. You can login to your account." });
            }
            catch (Exception)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Something went wrong. Please try again." });
            }
        }
    }

    [Route("RegisterAdmin")]
    [HttpPost]
    public async Task<IActionResult> RegisterAdmin(string email, string password)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        var userExists = await _userManager.FindByEmailAsync(email);
        if(userExists != null)
        {
            return BadRequest(new {message="email already exists"});
        }
        try
        {
            var user = new UserModel { UserName = "admin", Email = email, PhoneNumber = "76612235" };
            var result = await _userManager.CreateAsync(user,password);
            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            return Ok(new { message = "Admin created successfully. You can login to your account." });
        }
        catch (Exception ex)
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
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var accessToken = await _tokenService.GenerateAccessTokenAsync(model.Email);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _identityContext.SaveChanges();

            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
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

            return new ObjectResult(
                new AuthenticationResponse
                    {
                        AccessToken = accessToken, 
                        RefreshToken = refreshToken
                    }
                );
        }
        else
        {
            return BadRequest(new {message = "Wrong password"});
        }    
    }

    [Route("logout")]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        KeyValuePair<string, string> refreshPair = Request.Cookies.FirstOrDefault(x => x.Key=="refreshToken");
        KeyValuePair<string, string> accessPair = Request.Cookies.FirstOrDefault(x => x.Key=="accessToken");
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessPair.Value);
        var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await _userManager.FindByIdAsync(userId);
        if(user.RefreshToken != refreshPair.Value)
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
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        _identityContext.SaveChanges();

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
        
        
        return Ok(new {message="Log out Successfully"});
    }

}