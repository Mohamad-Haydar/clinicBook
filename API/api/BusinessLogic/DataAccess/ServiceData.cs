using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Responce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        public async Task CreateServiceAsync(string serviceName)
        {
            const string cacheKey = "services";
            try
            {
                var res = await _appDbContext.Services.AddAsync(new ServiceModel() { ServiceName = serviceName }) ?? throw new BusinessException();
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task UpdateServiceAsync(ServiceModel model)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(model.Id).ConfigureAwait(false) ?? throw new BusinessException();
                service.ServiceName = model.ServiceName;
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task DeleteServiceAsync(int id)
        {
            try
            {
                var resDetail = await (from ds in _appDbContext.DoctorServices where ds.ServiceId == id
                                       join rd in _appDbContext.ReservationDetailModels
                                       on ds.ServiceId equals rd.DoctorServiceId    
                                       select rd).AnyAsync();
                if(resDetail)
                {
                    throw new BusinessException("عذرا, لا يمكن حذف هذه الخدمة حاليا, هناك من يستخدمها.");
                }
                var service = await _appDbContext.Services.FindAsync(id);
                _appDbContext.Services.Remove(service);
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }
}
