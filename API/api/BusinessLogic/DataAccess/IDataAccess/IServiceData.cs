using api.Models;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IServiceData
    {
        Task<IEnumerable<ServiceModel>> GetAllServicesAsync();
    }
}