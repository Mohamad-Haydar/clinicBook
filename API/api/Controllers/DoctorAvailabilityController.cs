using System.ComponentModel.DataAnnotations;
using api.Attributes;
using api.Data;
using api.Helper;
using api.library.Models.Request;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin, Roles.Secretary, Roles.Doctor)]
[Route("api/[controller]")]
public class DoctorAvailabilityController : Controller
{
    private readonly ApplicationDbContext _appDbContext;

    public DoctorAvailabilityController(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    [Route("availableDates")]
    [AllowAnonymous]
    public IActionResult GetAvailableDates(string id)
    {
        if(!ModelState.IsValid)
            return BadRequest(new {message="doctor not found"});
        
        var doctoravailabilities = _appDbContext.DoctorAvailabilities
                                    .Where(x => x.DoctorId == id)
                                    .Select(x => new {
                                        id = x.Id,
                                        day = new DateOnly(x.AvailableDate.Year,x.AvailableDate.Month, x.AvailableDate.Day), 
                                        dayName = x.DayName,
                                        startHour = CalcTime.GetTime(x.StartHour), 
                                        endHour = CalcTime.GetTime(x.EndHour),
                                        maxClient = x.MaxClient
                                    });
        return Ok(doctoravailabilities);
    }

    [HttpPost]
    [Route("openavailabledate")]
    public async Task<IActionResult> OpenAvailableDate([FromBody] OpenAvailableDateRequest model)
    {
        if(!ModelState.IsValid || model == null)
            return BadRequest(new {message="doctor not found"});
        
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
            var res = await _appDbContext.DoctorAvailabilities.AddAsync(available);
            if(res == null)
            {
                return BadRequest(new {message = "Please try again"});
            }
            await _appDbContext.SaveChangesAsync();
            
            return Ok(new {message = "Available date added successfully"});
        }
        catch (Exception ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        
    }

    [HttpPatch]
    [Route("updateAvailableDate")]
    public async Task<IActionResult> UpdateAvailableDate([FromBody] OpenAvailableDateRequest model)
    {
        if(!ModelState.IsValid || model == null)
            return BadRequest(new {message="please enter valid data"});

        var existedAvailability = await _appDbContext.DoctorAvailabilities.FirstOrDefaultAsync(x => x.Id == model.Id);
        if(existedAvailability == null)
        {
            return BadRequest(new {message="their is no available date to remove"});
        }
        if(model.StartHour > model.EndHour)
        {
            return BadRequest(new {message="check your dates start date should be less that end date"});
        }
        if(model.AvailableDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return BadRequest(new {message="check availability date should not be previouse today"});
        }

        existedAvailability.AvailableDate = model?.AvailableDate != null ? model.AvailableDate : existedAvailability.AvailableDate;
        existedAvailability.DayName = model?.AvailableDate != null ?  model.AvailableDate.DayOfWeek.ToString() : existedAvailability.AvailableDate.DayOfWeek.ToString();
        existedAvailability.StartHour = model?.StartHour != null ? model.StartHour : existedAvailability.StartHour;
        existedAvailability.EndHour = model?.EndHour != null ? model.EndHour : existedAvailability.EndHour;
        existedAvailability.MaxClient = model.MaxClient != 0 ? model.MaxClient : existedAvailability.MaxClient;

        try
        {
            await _appDbContext.SaveChangesAsync();
            
            return Ok(new {message = "Available date added successfully"});
        }
        catch (Exception ex)
        {
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }

    [HttpDelete]
    [Route("deleteAvailableDate")]
    public async Task<IActionResult> DeleteAvailableDate([Required] int id)
    {
        if(!ModelState.IsValid)
            return BadRequest(new{message="Please select valid date to remove"});
        
        var availableDate = await _appDbContext.DoctorAvailabilities.FirstOrDefaultAsync(x => x.Id == id);
        if(availableDate == null)
        {
            return Ok(new {message="Nothing to delete"});
        }
        var available = _appDbContext.Remove(availableDate);
        await _appDbContext.SaveChangesAsync();

        return Ok(new {message="availabel date removed successfully"});
    }

}