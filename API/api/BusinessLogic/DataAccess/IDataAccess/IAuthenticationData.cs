using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IAuthenticationData
    {
        Task<Result<AuthenticationResponse>> LoginUserAsync(LoginRequest model);
        Task RegisterAdminAsync(string email, string password);
        Task<Result<AuthenticationResponse>> RegisterClientAsync(CreateUserRequest model);
        Task RegisterDoctorAsync(CreateDoctorRequest model);
        Task RegisterSecretaryAsync(CreateSecretaryRequest model);
        Task LogoutAsync(string refreshToken, string accessToken);
        Task<Result> ResetPasswordAsync(string uid, string token, string newPassword);
        Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<Result> ForgotPasswordAsync(string email);
        Task<Result<AuthenticationResponse>> UpdateUserAsync(UpdateUserRequest model);
    }
}