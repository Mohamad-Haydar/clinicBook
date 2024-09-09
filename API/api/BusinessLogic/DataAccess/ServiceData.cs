using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic.DataAccess
{
    public class ServiceData : IServiceData
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ILogger<ServiceData> _logger;

        public ServiceData(ApplicationDbContext appDbContext, ILogger<ServiceData> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ServiceModel>> GetAllServicesAsync()
        {
            try
            {
                var result = await _appDbContext.Services.ToListAsync().ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }
}
