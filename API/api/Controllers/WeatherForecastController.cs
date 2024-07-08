using api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public WeatherForecastController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("GetAllSecretaries")]
    public async Task<IActionResult> GetSecretaries()
    {
        // var users = await _context.Secretaries.ToListAsync();
        var allUsers = await _userManager.FindByEmailAsync("email@gmail.com");
        return Ok(allUsers);
    }
}
