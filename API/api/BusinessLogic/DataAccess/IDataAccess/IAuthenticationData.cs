using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IAuthenticationData
    {
        Task<AuthenticationResponse> LoginUserAsync(LoginRequest model);
        Task RegisterAdminAsync(string email, string password);
        Task RegisterClientAsync(CreateUserRequest model);
        Task RegisterDoctorAsync(CreateDoctorRequest model);
        Task RegisterSecretaryAsync(CreateSecretaryRequest model);
        Task LogoutAsync(KeyValuePair<string, string> refreshPair, KeyValuePair<string, string> accessPair);
    }
}