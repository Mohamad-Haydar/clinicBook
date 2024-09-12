using api.Models;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IServiceData
    {
        Task<IEnumerable<ServiceModel>> GetAllServicesAsync();
        Task UpdateServiceAsync(ServiceModel model);
        Task DeleteServiceAsync(int id);
        Task CreateServiceAsync(string serviceName);
    }
}