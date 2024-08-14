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
            return Ok(new Response("Service Added Successfully to the doctor"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("check your dates start date should be less that end date, and check availability date should not be previouse today"));
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
            return Ok(new Response("All Services added successfully"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("check your dates start date should be less that end date, and check availability date should not be previouse today"));
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
            return Ok(new Response("Service Duration updated successfully"));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again."));
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
            return Ok(new Response("Service Deleted successfully"));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again."));
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
            return Ok(new Response("doctor removed successfully"));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again."));
        }
    }

    [HttpPatch]
    [Route("updateDoctorInfo")]
    [AuthorizeRoles(Roles.Doctor,Roles.Admin, Roles.Secretary)]
    public async Task<IActionResult> UpdateDoctorInfo([FromBody] UpdateDoctorRequest model)
    {
        if(!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }
        try
        {
            await _doctorManagementData.UpdateDoctorInfoAsync(model);
            return Ok(new Response("Doctor Data updated successfully." ));
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again."));
        }
    }

    [HttpGet]
    [Route("getDoctorByEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorByEmail([Required] string email)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter the email"});
        }
        try
        {
            var doctor = await _doctorManagementData.GetDoctorByEmailAsync(email);
            return Ok(doctor);
        }
        catch (NotFoundException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (BusinessException ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again."));
        }
    }

    [HttpGet]
    [Route("GetDoctorById")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorById([Required] string id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response("Please enter valid input"));
        }

        try
        {
            var doctor = await _doctorManagementData.GetDoctorByIdAsync(id);
            return Ok(doctor);
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again"));
        }
    }

    [HttpGet]
    [Route("GetAllDoctors")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDoctors()
    {
        try
        {
            var doctors = await _doctorManagementData.GetAllDoctorsAsync();
            return doctors.Count() > 0 ? Ok(doctors) : NoContent();
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again"));
        }
    }

    [HttpGet]
    [Route("GetDoctorsByCategory")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorsByCategory([Required] int CategoryId)
    {
        try
        {
            var doctors = await _doctorManagementData.GetDoctorsByCategoryAsync(CategoryId);
            return doctors.Count() > 0 ? Ok(doctors) : NoContent();
        }
        catch (Exception)
        {
            return BadRequest(new Response("Something whent wrong, please try again"));
        }
    }
}