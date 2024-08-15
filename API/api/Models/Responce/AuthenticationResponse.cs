namespace api.Models.Responce;

public class AuthenticationResponse
{
    public string Id {get;set;}
    public string UserName {get;set;}
    public string Email {get;set;}
    public string PhoneNumber {get;set;}
    public string AccessToken {get;set;}
    public string RefreshToken {get;set;}
    public IEnumerable<string> Roles {get;set;}
}