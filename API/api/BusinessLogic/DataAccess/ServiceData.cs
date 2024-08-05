using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic.DataAccess
{
    public class ServiceData : IServiceData
    {
        private readonly ApplicationDbContext _appDbContext;

        public ServiceData(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<ServiceModel>> GetAllServicesAsync()
        {
            try
            {
                var result = await _appDbContext.Services.ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
