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
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorManagementData.AddDoctorServiceAsync(doctorService);
            return Ok(new Response("تم اضافة خدمة بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpPost]
    [Route("addMultipleService")]
    public async Task<IActionResult> AddMultipleService([FromBody] List<DoctorServiceRequest> doctorServices)
    {
         if(!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorManagementData.AddMultipleServiceAsync(doctorServices);
            return Ok(new Response("تم اضافة خدمات بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
        
    }

    [HttpPatch]
    [Route("updateDoctorServiceDuration")]
    public async Task<IActionResult> UpdateDoctorServiceDuration([Required] int id, [Required] int duration)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorManagementData.UpdateDoctorServiceDurationAsync(id, duration);
            return Ok(new Response("تم تحديث التوقيت بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpDelete]
    [Route("deleteDoctorService")]
    public async Task<IActionResult> DeleteDoctorService([Required] int id)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorManagementData.DeleteDoctorServiceAsync(id);
            return Ok(new Response("تم ازالة الخدمة بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpDelete]
    [Route("removeDoctor")]
    public async Task<IActionResult> RemoveDoctor([Required] string id)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorManagementData.RemoveDoctorAsync(id);
            return Ok(new Response("تم ازالة الدكتور بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
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
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorManagementData.UpdateDoctorInfoAsync(model);
            return Ok(new Response("تم تحديث معلومات الدكتور بنجاح" ));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("getDoctorByEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorByEmail([Required] string email)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var doctor = await _doctorManagementData.GetDoctorByEmailAsync(email);
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetDoctorById")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorById([Required] string id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }

        try
        {
            var doctor = await _doctorManagementData.GetDoctorByIdAsync(id);
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
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
            return doctors.Any() ? Ok(doctors) : NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
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
            return doctors.Any() ? Ok(doctors) : NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpPost]
    [Route("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            string path = await _doctorManagementData.UploadImageAsync(file);
            return Ok(new Response(path));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }
}