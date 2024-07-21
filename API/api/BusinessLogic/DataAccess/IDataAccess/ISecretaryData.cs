using api.Models;
using api.Models.Request;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface ISecretaryData
    {
        Task<SecretaryModel> GetSecretariebyEmailAsync(string email);
    }
}