using System.ComponentModel.DataAnnotations;
using api.Models.Request;
using Microsoft.AspNetCore.Mvc;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models.Responce;
using static System.Runtime.InteropServices.JavaScript.JSType;
using api.Attributes;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using api.Exceptions;
using api.Helper;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin, Roles.Secretary, Roles.Doctor)]
[Route("api/[controller]")]
public class DoctorAvailabilityController : Controller
{
    private readonly IDoctorAvailabilityData _doctorAvailabilityData;
    private readonly IMessageProvider _messageProvider;

    public DoctorAvailabilityController(IDoctorAvailabilityData doctorAvailabilityData, IMessageProvider messageProvider)
    {
        _doctorAvailabilityData = doctorAvailabilityData;
        _messageProvider = messageProvider;
    }

    [HttpGet]
    [Route("availableDates")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableDates([Required] string id)
    {
       if (!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var result = await _doctorAvailabilityData.GetAvailableDatesAsync(id).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPost]
    [Route("openavailabledate")]
    public async Task<IActionResult> OpenAvailableDate([FromBody] OpenAvailableDateRequest model)
    {
        if (!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorAvailabilityData.OpenAvailableDateAsync(model).ConfigureAwait(false);   
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPatch]
    [Route("updateAvailableDate")]
    public async Task<IActionResult> UpdateAvailableDate([FromBody] UpdateAvailableDateRequest model)
    {
        if (!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorAvailabilityData.UpdateAvailableDateAsync(model).ConfigureAwait(false);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpDelete]
    [Route("deleteAvailableDate")]
    public async Task<IActionResult> DeleteAvailableDate([Required] int id)
    {
        if (!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            await _doctorAvailabilityData.DeleteAvailableDateAsync(id).ConfigureAwait(false);
            return Ok(Result.Success(_messageProvider.GetMessage("deleteAvailableDateSuccess")));
        }
        catch (BusinessException ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
        catch (Exception ex)
        {
            if (ex.Message.StartsWith("MYERROR:"))
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage(ex.Message[8..])));
            }
             return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetAllDoctorAvailabilities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDoctorAvailabilities([Required] string doctorId)
    {
        try
        {
            var res = await _doctorAvailabilityData.GetAllDoctorAvailabilitiesAsync(doctorId).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetDoctorAvailabilitiesOfDay")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorAvailabilitiesOfDay(DateOnly date)
    {
        try
        {
            var res = await _doctorAvailabilityData.GetDoctorAvailabilitiesOfDayAsync(date).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPost]
    [Route("CreateRepeatedAvailability")]
    public async Task<IActionResult> CreateRepeatedAvailability([FromBody] IEnumerable<OpenAvailableDateRequest> model)
    {
        if (!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _doctorAvailabilityData.OpenRepeatedAvailableDateAsync(model).ConfigureAwait(false);
            return res.IsSuccess ? Ok(res) : BadRequest(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }
}