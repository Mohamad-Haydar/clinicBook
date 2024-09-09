using System.ComponentModel.DataAnnotations;
using api.Models.Request;
using Microsoft.AspNetCore.Mvc;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models.Responce;
using static System.Runtime.InteropServices.JavaScript.JSType;
using api.Attributes;
using api.Models;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin, Roles.Secretary, Roles.Doctor)]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableDates([Required] string id)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BadRequestResponse());
        
        try
        {
            var result = await _doctorAvailabilityData.GetAvailableDatesAsync(id).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpPost]
    [Route("openavailabledate")]
    public async Task<IActionResult> OpenAvailableDate([FromBody] OpenAvailableDateRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorAvailabilityData.OpenAvailableDateAsync(model).ConfigureAwait(false);   
            return Ok(new Response("تم انشاء تاريخ بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpPatch]
    [Route("updateAvailableDate")]
    public async Task<IActionResult> UpdateAvailableDate([FromBody] UpdateAvailableDateRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorAvailabilityData.UpdateAvailableDateAsync(model).ConfigureAwait(false);
            return Ok(new Response ("لقد تم تحديث التاؤيخ بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response (ex.Message));
        }
    }

    [HttpDelete]
    [Route("deleteAvailableDate")]
    public async Task<IActionResult> DeleteAvailableDate([Required] int id)
    {
        if(!ModelState.IsValid)
            return BadRequest(new BadRequestResponse());

        try
        {
            await _doctorAvailabilityData.DeleteAvailableDateAsync(id).ConfigureAwait(false);
            return Ok(new Response("لقد تم حذف الموعد و اعادة ترتيب الحجوزات بنجاح"));
        }
        catch (Exception ex)
        {
            if (ex.Message.StartsWith("MYERROR:"))
            {
                return BadRequest(new Response(ex.Message[8..]));
            }
            return BadRequest(new BadRequestResponse());
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
            return BadRequest(new Response(ex.Message));
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
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpPost]
    [Route("CreateRepeatedAvailability")]
    public async Task<IActionResult> CreateRepeatedAvailability([FromBody] IEnumerable<OpenAvailableDateRequest> model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _doctorAvailabilityData.OpenRepeatedAvailableDateAsync(model).ConfigureAwait(false);
            return Ok(new Response("تم انشاء تاريخ بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }
}