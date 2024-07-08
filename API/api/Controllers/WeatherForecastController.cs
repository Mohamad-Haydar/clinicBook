using api.Data;
using api.library.DataAccess;
using api.library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly UserManager<IdentityUser> _userManager;
    public WeatherForecastController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("GetAllSecretaries")]
    public List<SecretaryModel> GetSecretaries()
    {
        SecretaryData secretaryData = new(_configuration);

        return secretaryData.GetSecretaries();

    }
}
