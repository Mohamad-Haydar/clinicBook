using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic.DataAccess;

public class DoctorAvailabilityData : IDoctorAvailabilityData
{
    private readonly UserManager<UserModel> _userManager;
    private readonly ApplicationDbContext _appDbContext;
    private readonly Dictionary<string, string> Days;

    public DoctorAvailabilityData(UserManager<UserModel> userManager,
                              ApplicationDbContext appDbContext)
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
        Days = new()
        {
             { "Monday", "الاثنين" },
             { "Tuesday", "الثلاثاء" },
             { "Wednesday", "الاربعاء" },
             { "Thursday", "الخميس" },
             { "Friday", "الجمعة" },
             { "Saturday", "السبت" },
             { "Sunday", "الاحد" }
        };
    }

    public async Task<IEnumerable<DoctorAvailabilityResponse>> GetAvailableDatesAsync(string id)
    {
        try
        {
            var doctoravailabilities = await _appDbContext.DoctorAvailabilities
                                   .Where(x => x.DoctorId == id)
                                   .Select(x => new DoctorAvailabilityResponse
                                   {
                                       id = x.Id,
                                       day = new DateOnly(x.AvailableDate.Year, x.AvailableDate.Month, x.AvailableDate.Day),
                                       dayName = x.DayName,
                                       startHour = x.StartHour,
                                       endHour = x.EndHour,
                                       maxClient = x.MaxClient
                                   }).ToListAsync();
            return doctoravailabilities;
        }
        catch (Exception)
        {
            throw new BusinessException();
        }
    }

    public async Task OpenAvailableDateAsync(OpenAvailableDateRequest model)
    {
        List<DoctorAvailabilityModel> availables = [];
        for (int i = 0; i < model.NbOfOpenAvailability; i++)
        {
            availables.Add(
                new()
                {
                    AvailableDate = model.AvailableDate.AddDays(7*i) ,
                    DayName = Days[model.AvailableDate.DayOfWeek.ToString()],
                    StartHour = model.StartHour,
                    EndHour = model.EndHour,
                    MaxClient = model.MaxClient,
                    DoctorId = model.DoctorId,
                    RepetitionDelay = model.RepetitionDelay,
                    NbOfOpenAvailability = model.NbOfOpenAvailability,
                }
            );
        }
        try
        {
            var doc = await _userManager.FindByIdAsync(model.DoctorId) ?? throw new UserNotFoundException();
            if (availables == null)
            {
                throw new FailedToAddException();
            }
            await _appDbContext.DoctorAvailabilities.AddRangeAsync(availables);
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task OpenRepeatedAvailableDateAsync(IEnumerable<OpenAvailableDateRequest> model)
    {
        string doctorId = "";
        List<DoctorAvailabilityModel> availables = [];
        foreach (var item in model)
        {
            doctorId = item.DoctorId;
            availables.Add(
                new()
                {
                    AvailableDate = item.AvailableDate,
                    DayName = Days[item.AvailableDate.DayOfWeek.ToString()],
                    StartHour = item.StartHour,
                    EndHour = item.EndHour,
                    MaxClient = item.MaxClient,
                    DoctorId = item.DoctorId,
                    RepetitionDelay = item.RepetitionDelay,
                }
            );
        }
        
        try
        {
            var doc = await _userManager.FindByIdAsync(doctorId) ?? throw new UserNotFoundException();
            if(availables == null)
            {
                throw new FailedToAddException();
            }
            await _appDbContext.DoctorAvailabilities.AddRangeAsync(availables);
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }


    public async Task UpdateAvailableDateAsync(UpdateAvailableDateRequest model)
    {
        var existedAvailability = await _appDbContext.DoctorAvailabilities.FirstOrDefaultAsync(x => x.Id == model.Id) ?? throw new UserNotFoundException();
        if (model.StartHour > model.EndHour)
        {
            throw new InvalidDataException("انتبه, يجب ان تكون ساعة البدء قبل ساعة الانتهاء");
        }
        if (model.AvailableDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new InvalidDataException("انتبه, يجب ان يكون التاريخ في المستقبل ");
        }

        existedAvailability.AvailableDate = model?.AvailableDate != null ? model.AvailableDate : existedAvailability.AvailableDate;
        existedAvailability.DayName = model?.AvailableDate != null ? model.AvailableDate.DayOfWeek.ToString() : existedAvailability.AvailableDate.DayOfWeek.ToString();
        existedAvailability.StartHour = model?.StartHour != null ? model.StartHour : existedAvailability.StartHour;
        existedAvailability.EndHour = model?.EndHour != null ? model.EndHour : existedAvailability.EndHour;
        existedAvailability.MaxClient = model.MaxClient != 0 ? model.MaxClient : existedAvailability.MaxClient;

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DeleteAvailableDateAsync(int id)
    {
        var availableDate = await _appDbContext.DoctorAvailabilities.FirstOrDefaultAsync(x => x.Id == id) ?? throw new UserNotFoundException();
        try
        {
            var available = _appDbContext.Remove(availableDate);
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new BusinessException();
        }
    }

    public async Task<IEnumerable<DoctorAvailabilityModel>> GetAllDoctorAvailabilitiesAsync(string doctorId)
    {
        try
        {
            var res = await _appDbContext.DoctorAvailabilities.Where(x => x.DoctorId == doctorId).ToListAsync();
            return res;
        }
        catch (Exception)
        {
            throw new BusinessException();
        }
    }

    public async Task<IEnumerable<DoctorAvailabilityModel>> GetDoctorAvailabilitiesOfDayAsync(DateOnly date){
        try
        {
             if(date == DateOnly.MinValue)
            {
                var res = await _appDbContext.DoctorAvailabilities.Where(x => x.AvailableDate == DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
                return res;
            }else{
               var res = await _appDbContext.DoctorAvailabilities.Where(x => x.AvailableDate == date).ToListAsync();
                return res;
            }

            return [];
        }
        catch (Exception)
        {
            throw new BusinessException();
        }
    }
}