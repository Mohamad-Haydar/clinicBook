using System.ComponentModel.DataAnnotations;
using api.Attributes;
using api.Data;
using api.Helper;
using api.Models.Request;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;
using api.BusinessLogic.DataAccess.IDataAccess;

namespace api.Controllers;

// [AuthorizeRoles(Roles.Admin, Roles.Secretary, Roles.Doctor)]
[Route("api/[controller]")]
public class DoctorAvailabilityController : Controller
{
    private readonly IDoctorAvailabilityData _doctorAvailabilityData;

    public DoctorAvailabilityController(IDoctorAvailabilityData doctorAvailabilityData)
    {
        _doctorAvailabilityData = doctorAvailabilityData;
    }

    [HttpGet]
    [Route("availableDates")]
    public IActionResult GetAvailableDates([Required] string id)
    {
        if(!ModelState.IsValid)
            return BadRequest(new {message="doctor not found"});
        
        try
        {
            var result = _doctorAvailabilityData.GetAvailableDates(id);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Something went wrong. Please try again." });
        }
    }

    [HttpPost]
    [Route("openavailabledate")]
    public async Task<IActionResult> OpenAvailableDate([FromBody] OpenAvailableDateRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        try
        {
            await _doctorAvailabilityData.OpenAvailableDateAsync(model);   
            return Ok(new {message = "Available date added successfully"});
        }
         catch(NotFoundException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
        catch(BusinessException ex)
        {
            return BadRequest(new {message = ex.Message});
        }
        catch (Exception ex)
        {
            return BadRequest(new {message="something when wrong please try again"});
        }
        
    }

    [HttpPatch]
    [Route("updateAvailableDate")]
    public async Task<IActionResult> UpdateAvailableDate([FromBody] UpdateAvailableDateRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        try
        {
            await _doctorAvailabilityData.UpdateAvailableDateAsync(model);
            return Ok(new {message = "Available date added successfully"});
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (BusinessException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (Exception)
        {
            return BadRequest(new {message="Something when wrong, please try again"});
        }
    }

    [HttpDelete]
    [Route("deleteAvailableDate")]
    public async Task<IActionResult> DeleteAvailableDate([Required] int id)
    {
        if(!ModelState.IsValid)
            return BadRequest(new { message = "Please select valid date to remove" });

        try
        {
            await _doctorAvailabilityData.DeleteAvailableDateAsync(id);
            return Ok(new {message="availabel date removed successfully"});
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
         catch (BusinessException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (Exception)
        {
            return BadRequest(new {message="something when wrong please try again"});
        }
    }

}