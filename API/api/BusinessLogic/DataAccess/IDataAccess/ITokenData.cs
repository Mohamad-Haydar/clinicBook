using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface ITokenData
    {
        Task<Result<AuthenticationResponse>> RefreshAsync(RefreshRequest tokenApiModel);
    }
}