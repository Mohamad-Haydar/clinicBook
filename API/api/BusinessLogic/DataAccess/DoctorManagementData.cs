using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Data;
using api.Exceptions;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.BusinessLogic.DataAccess;

public class DoctorManagementData : IDoctorManagementData
{
    private readonly ILogger<DoctorManagementData> _logger;
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly ISqlDataAccess _sql;
    private readonly ApplicationDbContext _appDbContext;
    private readonly UserManager<UserModel> _userManager;
    private readonly IdentityAppDbContext _identityContext;
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public DoctorManagementData(IOptions<ConnectionStrings> connectionStrings, ISqlDataAccess sql, ApplicationDbContext appDbContext, UserManager<UserModel> userManager, IdentityAppDbContext identityContext, ILogger<DoctorManagementData> logger, IMemoryCache cache, MemoryCacheEntryOptions cacheOptions)
    {
        _connectionStrings = connectionStrings;
        _sql = sql;
        _appDbContext = appDbContext;
        _userManager = userManager;
        _identityContext = identityContext;
        _logger = logger;
        _cache = cache;
        _cacheOptions = cacheOptions;
    }


    public async Task AddDoctorServiceAsync(DoctorServiceRequest data)
    {
        try
        {
            await _sql.SaveDataAsync<DoctorServiceRequest>("sp_insert_doctor_service", data, _connectionStrings.Value.AppDbConnection).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }
    public async Task AddMultipleServiceAsync(List<DoctorServiceRequest> doctorServices)
    {
        using (var transaction = _appDbContext.Database.BeginTransaction())
        {
            try
            {
                foreach (var doctorService in doctorServices)
                {
                    await AddDoctorServiceAsync(doctorService).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }

    public async Task UpdateDoctorServiceDurationAsync(int id, int duration)
    {
        try
        {
            var service = await _appDbContext.DoctorServices.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false) ?? throw new UserNotFoundException();
            service.Duration = duration;
            await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    //TODO: when deleting i should delete the reservation detail 
    // and check if i want to delete the client reservation
    public async Task DeleteDoctorServiceAsync(int id)
    {
        var service = await _appDbContext.DoctorServices.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false) ?? throw new UserNotFoundException();
        try
        {
            var res = _appDbContext.DoctorServices.Remove(service);
            await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task RemoveDoctorAsync(string id)
    {
        var doctor = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false) ?? throw new UserNotFoundException();
        var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false) ?? throw new UserNotFoundException();
        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                _appDbContext.Remove(doctor);
                await _userManager.DeleteAsync(user).ConfigureAwait(false);
                await _identityContext.SaveChangesAsync().ConfigureAwait(false);
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
            }
            catch (UserNotFoundException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }

    public async Task UpdateDoctorInfoAsync(UpdateDoctorRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false) ?? throw new UserNotFoundException();
        var doctor = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.Email == model.Email).ConfigureAwait(false) ?? throw new UserNotFoundException();

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                string userName = model.FirstName ?? doctor.FirstName + model.LastName ?? doctor.LastName;
                user.UserName = userName;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

                doctor.FirstName = model.FirstName ?? doctor.FirstName;
                doctor.LastName = model.LastName ?? doctor.LastName;
                doctor.PhoneNumber = model.PhoneNumber ?? doctor.PhoneNumber;
                doctor.Description = model.Description ?? doctor.Description;
                doctor.CategoryId = model.CategoryId > 0 ? model.CategoryId : doctor.CategoryId;
                doctor.Image = model.Image  ?? doctor.Image;

                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);
                await _identityContext.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
            }
            catch (UserNotFoundException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }

    public async Task<DoctorInfoResponse> GetDoctorByEmailAsync(string email)
    {
        try
        {
            var doctor = await (from d in _appDbContext.Doctors
                    where d.Email == email
                    join c in _appDbContext.Categories on d.CategoryId equals c.Id
                    select new DoctorInfoResponse { Id = d.Id, FirstName = d.FirstName, LastName = d.LastName, Email = d.Email, PhoneNumber = d.PhoneNumber, Description = d.Description, CategoryName = c.CategoryName, Image = d.Image }).FirstOrDefaultAsync().ConfigureAwait(false);
            if (doctor == null)
            {
                throw new UserNotFoundException();
            }

            return doctor;
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task<DoctorInfoResponse> GetDoctorByIdAsync(string id)
    {
        try
        {
            var doctor = await (from d in _appDbContext.Doctors where d.Id == id
                                join c in _appDbContext.Categories on d.CategoryId equals c.Id
                                select new DoctorInfoResponse
                                {
                                    Id = d.Id,
                                    FirstName = d.FirstName,
                                    LastName = d.LastName,
                                    Email = d.Email,
                                    PhoneNumber = d.PhoneNumber,
                                    Description = d.Description,
                                    CategoryName = c.CategoryName,
                                    Image = d.Image,
                                    Services = (from ds in _appDbContext.DoctorServices where ds.DoctorId == d.Id
                                                join ser in _appDbContext.Services on ds.ServiceId equals ser.Id
                                                select new DoctorServiceResponse
                                                {
                                                    Id = ds.Id,
                                                    ServiceName = ser.ServiceName,
                                                    Duration = ds.Duration,
                                                    DoctorId = ds.DoctorId,
                                                    ServiceId = ds.ServiceId
                                                }).ToList()
                                }).FirstOrDefaultAsync().ConfigureAwait(false);
            if (doctor == null)
            {
                throw new UserNotFoundException("هذا الطبيب غير موجود!");
            }
            
            return doctor;
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task<IEnumerable<DoctorInfoResponse>> GetAllDoctorsAsync()
    {
        string cacheKey = "doctors";
        try
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<DoctorInfoResponse>? doctors))
            {
                doctors = await _appDbContext.Doctors.AsNoTracking()
                            .Include(d => d.Category)
                            .Select(d => new DoctorInfoResponse
                            {
                                Id = d.Id,
                                FirstName = d.FirstName,
                                LastName = d.LastName,
                                Email = d.Email,
                                PhoneNumber = d.PhoneNumber,
                                Description = d.Description,
                                CategoryName = d.Category.CategoryName,
                                Image = d.Image
                            }).ToListAsync().ConfigureAwait(false);
                _cache.Set(cacheKey, doctors, _cacheOptions);
            }
            return doctors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<DoctorInfoResponse>> GetDoctorsByCategoryAsync(int CategoryId)
    {
        string cacheKey = "doctors_category_"+ CategoryId;
        try
        {
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<DoctorInfoResponse>? doctors))
            {
                doctors = await (from c in _appDbContext.Categories
                              where c.Id == CategoryId
                              join d in _appDbContext.Doctors on c.Id equals d.CategoryId
                              select new DoctorInfoResponse { Id = d.Id, FirstName = d.FirstName, LastName = d.LastName, Email = d.Email, PhoneNumber = d.PhoneNumber, Description = d.Description, CategoryName = c.CategoryName, Image = d.Image }).ToListAsync().ConfigureAwait(false);
                _cache.Set(cacheKey, doctors, _cacheOptions);
            }
            return doctors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                throw new Exception("الرجاء اختيار صورة");
            string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

            var filePath = Path.Combine(_storagePath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return file.FileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException("لقد حدث خطأ اثناء رفع الصورة, الرجاء المحاولة مجددا.");
        }

      
    }
}