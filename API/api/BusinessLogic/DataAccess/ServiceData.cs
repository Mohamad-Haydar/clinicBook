using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Helper;
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
        private readonly IMessageProvider _messageProvider;

        public ServiceData(ApplicationDbContext appDbContext, ILogger<ServiceData> logger, IMessageProvider messageProvider)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _messageProvider = messageProvider;
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


        public async Task<Result> CreateServiceAsync(string serviceName)
        {
            const string cacheKey = "services";
            try
            {
                var res = await _appDbContext.Services.AddAsync(new ServiceModel() { ServiceName = serviceName });
                if(res == null) return Result.Failure(_messageProvider.GetMessage("error"));
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);

                return Result.Success(_messageProvider.GetMessage("createServiceAsync"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task<Result> UpdateServiceAsync(ServiceModel model)
        {
            try
            {
                var service = await _appDbContext.Services.FindAsync(model.Id).ConfigureAwait(false);
                if(service == null) return Result.Failure(_messageProvider.GetMessage("error"));

                service.ServiceName = model.ServiceName;
                await _appDbContext.SaveChangesAsync();
                return Result.Success(_messageProvider.GetMessage("updateServiceAsync"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }

        public async Task<Result> DeleteServiceAsync(int id)
        {
            try
            {
                var resDetail = await (from ds in _appDbContext.DoctorServices where ds.ServiceId == id
                                       join rd in _appDbContext.ReservationDetailModels
                                       on ds.ServiceId equals rd.DoctorServiceId    
                                       select rd).AnyAsync();
                if(resDetail)
                {
                    return Result.Failure(_messageProvider.GetMessage("cantRemoveServiceError"));
                }
                var service = await _appDbContext.Services.FindAsync(id);
                _appDbContext.Services.Remove(service);
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
                return Result.Success(_messageProvider.GetMessage("deleteServiceAsync"));
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
