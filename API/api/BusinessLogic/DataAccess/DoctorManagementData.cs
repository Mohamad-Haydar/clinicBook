using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace api.BusinessLogic.DataAccess;

public class DoctorManagementData : IDoctorManagementData
{

    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly ISqlDataAccess _sql;
    private readonly ApplicationDbContext _appDbContext;
    private readonly UserManager<UserModel> _userManager;
    private readonly IdentityAppDbContext _identityContext;

    public DoctorManagementData(IOptions<ConnectionStrings> connectionStrings, ISqlDataAccess sql, ApplicationDbContext appDbContext, UserManager<UserModel> userManager, IdentityAppDbContext identityContext)
    {
        _connectionStrings = connectionStrings;
        _sql = sql;
        _appDbContext = appDbContext;
        _userManager = userManager;
        _identityContext = identityContext;
    }


    public async Task AddDoctorServiceAsync(DoctorServiceRequest data)
    {
        try
        {
            await _sql.SaveDataAsync<DoctorServiceRequest>("sp_insert_doctor_service", data, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception)
        {
            throw new BusinessException("Something when wrong please try again");
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
                    await AddDoctorServiceAsync(doctorService);
                }

            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new BusinessException("Something when wrong, please try again");
            }
        }
    }

    public async Task UpdateDoctorServiceDurationAsync(int id, int duration)
    {
        try
        {
            var service = await _appDbContext.DoctorServiceModels.FirstOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                throw new NotFoundException("something whent wrong please check your input data");
            }
            service.Duration = duration;
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new BusinessException("Something when wrong, please try again");
        }
    }

    //TODO: when deleting i should delete the reservation detail 
    // and check if i want to delete the client reservation
    public async Task DeleteDoctorServiceAsync(int id)
    {
        var service = await _appDbContext.DoctorServiceModels.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException("Service not found");
        try
        {
            var res = _appDbContext.DoctorServiceModels.Remove(service);
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new BusinessException("Something when wrong, please try again");
        }
    }

    public async Task RemoveDoctorAsync(string id)
    {
        var doctor = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);
        var user = await _userManager.FindByIdAsync(id);
        if (doctor == null)
        {
            throw new NotFoundException("Doctor not found");
        }
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                _appDbContext.Remove(doctor);
                await _userManager.DeleteAsync(user);
                await _identityContext.SaveChangesAsync();
                await _appDbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new BusinessException("Something went wrong. Please try again.");
            }
        }
    }

    public async Task UpdateDoctorInfoAsync(CreateDoctorRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        var doctor = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.Email == model.Email);
        if (doctor == null)
        {
            throw new NotFoundException("Doctor not found");
        }
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

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

                await _appDbContext.SaveChangesAsync();
                await _identityContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new BusinessException("Something went wrong. Please try again.");
            }
        }
    }

    public async Task<DoctorInfoResponce> GetDoctorByEmailAsync(string email)
    {
        try
        {
            IQueryable<DoctorInfoResponce> doctor = from d in _appDbContext.Doctors
                                                    where d.Email == email
                                                    join c in _appDbContext.CategoryModels on d.CategoryId equals c.Id
                                                    select new DoctorInfoResponce { Id = d.Id, FirstName = d.FirstName, LastName = d.LastName, Email = d.Email, PhoneNumber = d.PhoneNumber, Description = d.Description, CategoryName = c.CategoryName, Image = d.Image };
            return await doctor.FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw new BusinessException("Something went wrong. Please try again.");
        }
    }

    public async Task<DoctorInfoResponce> GetDoctorByIdAsync(string id)
    {
        try
        {
            var doctor = from d in _appDbContext.Doctors
                         where d.Id == id
                         join c in _appDbContext.CategoryModels on d.CategoryId equals c.Id
                         select new DoctorInfoResponce { Id = d.Id, FirstName = d.FirstName, LastName = d.LastName, Email = d.Email, PhoneNumber = d.PhoneNumber, Description = d.Description, CategoryName = c.CategoryName, Image = d.Image };
            return await doctor.FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<DoctorNameResponse>> GetAllDoctorsNameAndIdAsync()
    {
        try
        {
            var doctors = await _appDbContext.Doctors.Select(x => new DoctorNameResponse { Id = x.Id, FirstName = x.FirstName , LastName = x.LastName}).ToListAsync();
            return doctors;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<DoctorInfoResponce>> GetDoctorsByCategoryAsync(int CategoryId)
    {
        try
        {
            var doctors = from c in _appDbContext.CategoryModels
                          where c.Id == CategoryId
                          join d in _appDbContext.Doctors on c.Id equals  d.CategoryId 
                          select new DoctorInfoResponce { Id = d.Id, FirstName = d.FirstName, LastName = d.LastName, Email = d.Email, PhoneNumber = d.PhoneNumber, Description = d.Description, CategoryName = c.CategoryName, Image = d.Image };
            return await doctors.ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

}