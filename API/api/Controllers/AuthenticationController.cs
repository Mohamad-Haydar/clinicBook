using api.Attributes;
using api.Data;
using api.library.Models;
using api.Models;
using api.Models.Responce;
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
    public async Task<IActionResult> RegisterClient([FromBody] ClientModel clientModel, string password)
    {
        var userExists = await _userManager.FindByEmailAsync(clientModel.Email);
        if(userExists != null)
        {
            return BadRequest(new {message="email already exists"});
        }

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                var user = new UserModel { UserName = clientModel.Email, Email = clientModel.Email, PhoneNumber = clientModel.PhoneNumber };
                var result = await _userManager.CreateAsync(user,password);
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
                ClientModel client = new(){
                    Id = user.Id,
                    FirstName = clientModel.FirstName,
                    LastName = clientModel.LastName,
                    Email = clientModel.Email,
                    PhoneNumber = clientModel.PhoneNumber,
                };
                await _appContext.Clients.AddAsync(client);
                await _appContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "Client created successfully. You can login to your account." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Something went wrong. Please try again." });
            }
        }
    }

    [Route("RegisterSecretary")]
    [HttpPost]
    public async Task<IActionResult> RegisterSecretary([FromBody] SecretaryModel secretaryModel, string password)
    {
        var userExists = await _userManager.FindByEmailAsync(secretaryModel.Email);
        if(userExists != null)
        {
            return BadRequest(new {message="email already exists"});
        }

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                var user = new UserModel { UserName = secretaryModel.Email, Email = secretaryModel.Email, PhoneNumber = secretaryModel.PhoneNumber };
                var result = await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, Roles.Secretary.ToString());
                SecretaryModel secretary = new(){
                    Id = user.Id,
                    FirstName = secretaryModel.FirstName,
                    LastName = secretaryModel.LastName,
                    Email = secretaryModel.Email,
                    PhoneNumber = secretaryModel.PhoneNumber,
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
    public async Task<IActionResult> RegisterDoctor([FromBody] DoctorModel doctorModel, string password)
    {
        var userExists = await _userManager.FindByEmailAsync(doctorModel.Email);
        if(userExists != null)
        {
            return BadRequest(new {message="email already exists"});
        }

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                var user = new UserModel { UserName = doctorModel.Email, Email = doctorModel.Email, PhoneNumber = doctorModel.PhoneNumber };
                var result = await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, Roles.Doctor.ToString());
                DoctorModel doctor = new(){
                    Id = user.Id,
                    FirstName = doctorModel.FirstName,
                    LastName = doctorModel.LastName,
                    Email = doctorModel.Email,
                    PhoneNumber = doctorModel.PhoneNumber,
                    Description = doctorModel.Description,
                    CategoryId = doctorModel.CategoryId
                };
                await _appContext.Doctors.AddAsync(doctor);
                await _appContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "Doctor created successfully. You can login to your account." });
            }
            catch (Exception ex)
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
    public async Task<IActionResult> LoginUser(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(await _userManager.CheckPasswordAsync(user, password))
        {
            var accessToken = await _tokenService.GenerateAccessTokenAsync(email);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _identityContext.SaveChanges();

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

    private async Task<bool> IsValidEmailAndPassword(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user == null)
        {
            return false;
        }
        return await _userManager.CheckPasswordAsync(user, password);
    }

    

}