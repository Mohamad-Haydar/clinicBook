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

    public DoctorAvailabilityData(UserManager<UserModel> userManager,
                              ApplicationDbContext appDbContext)
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
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
            throw new BusinessException("An error occurred while registering the client");
        }
    }

    public async Task OpenAvailableDateAsync(OpenAvailableDateRequest model)
    {
        DoctorAvailabilityModel available = new()
        {
            AvailableDate = model.AvailableDate,
            DayName = model.AvailableDate.DayOfWeek.ToString(),
            StartHour = model.StartHour,
            EndHour = model.EndHour,
            MaxClient = model.MaxClient,
            DoctorId = model.DoctorId
        };
        try
        {
            var doc = await _userManager.FindByIdAsync(model.DoctorId);
            if (doc == null)
            {
                throw new NotFoundException("Doctor Not Found please enter a valid doctor");
            }
            var res = await _appDbContext.DoctorAvailabilities.AddAsync(available);
            if (res == null)
            {
                throw new FailedToAddException("failed to create a date, please try again");
            }
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }

    }

    public async Task UpdateAvailableDateAsync(UpdateAvailableDateRequest model)
    {
        var existedAvailability = await _appDbContext.DoctorAvailabilities.FirstOrDefaultAsync(x => x.Id == model.Id);
        if (existedAvailability == null)
        {
            throw new NotFoundException("their is no available date to update");
        }
        if (model.StartHour > model.EndHour)
        {
            throw new InvalidDataException("check your dates start date should be less that end date");
        }
        if (model.AvailableDate < DateOnly.FromDateTime(DateTime.Now))
        {
            throw new InvalidDataException("check availability date should not be previouse today");
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
        var availableDate = await _appDbContext.DoctorAvailabilities.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException("Not Found, Enter a valid input");
        try
        {
            var available = _appDbContext.Remove(availableDate);
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}