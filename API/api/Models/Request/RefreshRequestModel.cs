namespace api.Models;

public class RefreshRequestModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}