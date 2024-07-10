using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Data;
using api.library.Models;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IdentityAppDbContext _identityContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _appContext;

    public AuthenticationController(IdentityAppDbContext identityContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext appContext)
    {
        _identityContext = identityContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _appContext = appContext;
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
                var user = new IdentityUser { UserName = clientModel.Email, Email = clientModel.Email };
                var result = await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
                ClientModel client = new(){
                    FirstName = clientModel.FirstName,
                    LastName = clientModel.LastName,
                    Email = clientModel.Email,
                    PhoneNumber = clientModel.PhoneNumber,
                };
                await _appContext.Clients.AddAsync(client);
                await _appContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "User created successfully. You can login to your account." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Something went wrong. Please try again." });
            }
        }
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> LoginUser(string email, string password)
    {
        if(await IsValidEmailAndPassword(email, password))
        {
            return new ObjectResult(await GenerateToken(email));
        }
        else
        {
            return BadRequest();
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

     private async Task<dynamic> GenerateToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new
                {
                    Access_Token = "no token",
                    email = ""
                };
            }
            var roles = from ur in _identityContext.UserRoles
                        join r in _identityContext.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("loreamipsumissomesecreatekeytobeused")),
                        SecurityAlgorithms.HmacSha256
                        )
                    ),
                new JwtPayload(claims)
                );

            var output = new
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = email
            };

            return output;
                    
        }
}