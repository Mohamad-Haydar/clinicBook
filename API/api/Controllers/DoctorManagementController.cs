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
using System.Diagnostics;
using api.Helper;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin, Roles.Secretary)]
[Route("api/[controller]")]
public class DoctorManagementController : ControllerBase
{
    private readonly IDoctorManagementData _doctorManagementData;
    private readonly ILogger<DoctorManagementController> _logger;
    private readonly IMessageProvider _messageProvider;

    public DoctorManagementController(IDoctorManagementData doctorManagementData, ILogger<DoctorManagementController> logger, IMessageProvider messageProvider)
    {
        _doctorManagementData = doctorManagementData;
        _logger = logger;
        _messageProvider = messageProvider;
    }

    [HttpPost]
    [Route("addDoctorService")]
    public async Task<IActionResult> AddDoctorService([FromBody] DoctorServiceRequest doctorService)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        try
        {
            await _doctorManagementData.AddDoctorServiceAsync(doctorService).ConfigureAwait(false);
            return Ok(Result.Success(_messageProvider.GetMessage("AddDoctorServiceSuccess")));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPost]
    [Route("addMultipleService")]
    public async Task<IActionResult> AddMultipleService([FromBody] List<DoctorServiceRequest> doctorServices)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            await _doctorManagementData.AddMultipleServiceAsync(doctorServices).ConfigureAwait(false);
            return Ok(Result.Success(_messageProvider.GetMessage("AddDoctorServiceSuccess")));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
        
    }

    [HttpPatch]
    [Route("updateDoctorServiceDuration")]
    public async Task<IActionResult> UpdateDoctorServiceDuration([Required] int id, [Required] int duration)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorManagementData.UpdateDoctorServiceDurationAsync(id, duration).ConfigureAwait(false);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpDelete]
    [Route("deleteDoctorService")]
    public async Task<IActionResult> DeleteDoctorService([Required] int id)
    {

        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorManagementData.DeleteDoctorServiceAsync(id).ConfigureAwait(false);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpDelete]
    [Route("removeDoctor")]
    public async Task<IActionResult> RemoveDoctor([Required] string id)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorManagementData.RemoveDoctorAsync(id).ConfigureAwait(false);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPatch]
    [Route("updateDoctorInfo")]
    [AuthorizeRoles(Roles.Doctor,Roles.Admin, Roles.Secretary)]
    public async Task<IActionResult> UpdateDoctorInfo([FromBody] UpdateDoctorRequest model)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorManagementData.UpdateDoctorInfoAsync(model).ConfigureAwait(false);
            return res.IsSuccess? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("getDoctorByEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorByEmail([Required] string email)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorManagementData.GetDoctorByEmailAsync(email).ConfigureAwait(false);
            return Ok(res.Data);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetDoctorById")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorById([Required] string id)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorManagementData.GetDoctorByIdAsync(id).ConfigureAwait(false);
            return Ok(res.Data);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetAllDoctors")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDoctors()
    {
        try
        {
            var doctors = await _doctorManagementData.GetAllDoctorsAsync().ConfigureAwait(false);
            return doctors.Any() ? Ok(doctors) : NoContent();
        }
        catch (Exception)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetDoctorsByCategory")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorsByCategory([Required] int CategoryId)
    {
        try
        {
            var doctors = await _doctorManagementData.GetDoctorsByCategoryAsync(CategoryId).ConfigureAwait(false);
            return doctors.Any() ? Ok(doctors) : NoContent();
        }
        catch (Exception)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPost]
    [Route("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            string path = await _doctorManagementData.UploadImageAsync(file).ConfigureAwait(false);
            return Ok(new Response(path));
        }
        catch(BusinessException ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage(ex.Message)));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }
}