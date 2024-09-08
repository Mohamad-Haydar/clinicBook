using Microsoft.AspNetCore.Identity;

namespace api.Models;


public class UserModel : IdentityUser
{
    public string? RefreshToken { get; set; } = "";
    public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.MinValue;
    public string? OldRefreshToken { get; set; } = "";
    public DateTime OldRefreshTokenExpiryTime { get; set; } = DateTime.MinValue;
}