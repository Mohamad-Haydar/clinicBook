using api.Models;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IServiceData
    {
        Task<IEnumerable<ServiceModel>> GetAllServicesAsync();
        Task<Result> UpdateServiceAsync(ServiceModel model);
        Task<Result> DeleteServiceAsync(int id);
        Task<Result> CreateServiceAsync(string serviceName);
    }
}