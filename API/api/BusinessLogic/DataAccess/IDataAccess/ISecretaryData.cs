using api.Models;
using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface ISecretaryData
    {
        Task<Result<SecretaryModel>> GetSecretariebyEmailAsync(string email);
    }
}