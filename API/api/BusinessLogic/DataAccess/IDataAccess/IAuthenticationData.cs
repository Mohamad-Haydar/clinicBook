using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IAuthenticationData
    {
        Task<AuthenticationResponse> LoginUserAsync(LoginRequest model);
        Task RegisterAdminAsync(string email, string password);
        Task<AuthenticationResponse> RegisterClientAsync(CreateUserRequest model);
        Task RegisterDoctorAsync(CreateDoctorRequest model);
        Task RegisterSecretaryAsync(CreateSecretaryRequest model);
        Task LogoutAsync(string refreshToken, string accessToken);
        Task ResetPasswordAsync(string uid, string token, string newPassword);
        Task ForgotPasswordAsync(string email);
        Task<AuthenticationResponse> UpdateUserAsync(UpdateUserRequest model);
    }
}