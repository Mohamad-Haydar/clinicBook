using Microsoft.AspNetCore.Identity;

namespace api.Models;


public class UserModel : IdentityUser
{
    public string? RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.MinValue;
}