using api.Attributes;
using api.Data;
using api.Models.Request;
using api.Models.Responce;
using api.Models;
using api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;
using System.ComponentModel.DataAnnotations;
using api.BusinessLogic.DataAccess.IDataAccess;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin, Roles.Secretary)]
[Route("api/[controller]")]
public class DoctorManagementController : ControllerBase
{
    private readonly IDoctorManagementData _doctorManagementData;
    public DoctorManagementController(IDoctorManagementData doctorManagementData)
    {
        _doctorManagementData = doctorManagementData;
    }

    [HttpPost]
    [Route("addDoctorService")]
    public async Task<IActionResult> AddDoctorService([FromBody] DoctorServiceRequest doctorService)
    {
        if(!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        try
        {
            await _doctorManagementData.AddDoctorServiceAsync(doctorService);
            return Ok(new {message="Service Added Successfully to the doctor"});
        }
        catch (BusinessException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (Exception)
        {
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }

    [HttpPost]
    [Route("addMultipleService")]
    public async Task<IActionResult> AddMultipleService([FromBody] List<DoctorServiceRequest> doctorServices)
    {
         if(!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        try
        {
            await _doctorManagementData.AddMultipleServiceAsync(doctorServices);
            return Ok(new {message="All Services added successfully"});
        }
        catch (BusinessException ex)
        {
            return BadRequest(new {message=ex.Message});
        }
        catch (Exception)
        {
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
        
    }

    [HttpPatch]
    [Route("updateDoctorServiceDuration")]
    public async Task<IActionResult> UpdateDoctorServiceDuration([Required] int id, [Required] int duration)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message = "please enter valid data"});
        }
        try
        {
            await _doctorManagementData.UpdateDoctorServiceDurationAsync(id, duration);
            return Ok(new {message="Service Duration updated successfully"});
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
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }

    [HttpDelete]
    [Route("deleteDoctorService")]
    public async Task<IActionResult> DeleteDoctorService([Required] int id)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message = "Please enter a valid input"});
        }
        try
        {
            await _doctorManagementData.DeleteDoctorServiceAsync(id);
            return Ok(new {message="Service Deleted successfully"});
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
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }

    [HttpDelete]
    [Route("removeDoctor")]
    public async Task<IActionResult> RemoveDoctor([Required] string id)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message = "Please enter valid input"});
        }
        try
        {
            await _doctorManagementData.RemoveDoctorAsync(id);
            return Ok(new {message="doctor removed successfully"});
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
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }

    [HttpPatch]
    [Route("updateDoctorInfo")]
    [AuthorizeRoles(Roles.Doctor,Roles.Admin, Roles.Secretary)]
    public async Task<IActionResult> UpdateDoctorInfo([FromBody] CreateDoctorRequest model)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest("Please enter valid input");
        }
        try
        {
            await _doctorManagementData.UpdateDoctorInfoAsync(model);
            return Ok(new { message = "Doctor Data updated successfully." });
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
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }

    [HttpGet]
    [Route("getDoctorInfo")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorInfo([Required] string email)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter the email"});
        }
        try
        {
            var doctor = await _doctorManagementData.GetDoctorInfoAsync(email);
            return Ok(doctor);
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
            return BadRequest(new {message="check your dates start date should be less that end date, and check availability date should not be previouse today"});
        }
    }


}