using System.ComponentModel.DataAnnotations;
using api.Data;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Helper;
using api.Models.Request;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models.Responce;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Net;
using api.Attributes;

namespace api.Controllers;

[Authorize]
[Route("/api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationData _reservationData;

    public ReservationController(IReservationData reservationData)
    {
        _reservationData = reservationData;
    }


    [HttpPost]
    [Route("CreateQueueReservation")]
    public async Task<IActionResult> CreateQueueReservation([FromBody] CreateQueueReservationRequest model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new BadRequestResponse());
        }

        try
        {
            await _reservationData.CreateQueueReservationAsync(model);
            return Ok(new Response("لقد تم حجز الموعد بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetReservationDetail")]
    public async Task<IActionResult> GetReservationDetails([Required] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var res = await _reservationData.GetReservationDetailsAsync(id);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetAllPersonalReservations")]
    public async Task<IActionResult> GetAllPersonalReservations([Required] string ClientId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var res = await _reservationData.GetAllPersonalReservationsAsync(ClientId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetConcurrentBookings")]
    public async Task<IActionResult> GetConcurrentBookings([Required] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var result = await _reservationData.GetConcurrentBookingsAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetPreviousBookings")]
    public async Task<IActionResult> GetPreviousBookings([Required] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var result = await _reservationData.GetPreviousBookingsAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }

    }

    [HttpDelete]
    [Route("DeleteSpecificReservation")]
    public async Task<IActionResult> DeleteSpecificReservation([Required] int clientReservationId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BadRequestResponse());

        try
        {
            await _reservationData.DeleteSpecificReservationAsync(clientReservationId);
            return Ok(new Response("لقد تم ازالة موعدك بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
            }


    }
    
    [HttpPatch]
    [Route("UpdateSpecificReservation")]
    public async Task<IActionResult> UpdateSpecificReservation([FromBody] UpdateReservationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _reservationData.UpdateSpecificReservationAsync(model);
            return Ok(new Response("لقد تم تحديث الحجز بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetAllReservationForTheDay")]
    public async Task<IActionResult> GetAllReservationForTheDay([Required] int DoctorAvailabilityId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var result = await _reservationData.GetAllReservationForTheDayAsync(DoctorAvailabilityId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpPost]
    [Route("MarkCompleteReservation")]
    public async Task<IActionResult> MarkCompleteReservation([Required] int ClientReservationId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            await _reservationData.MarkCompleteReservationAsync(ClientReservationId);
            return Ok(new Response("تم انهاء الزيارة"));
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

    [HttpGet]
    [Route("GetAllReservationOfAvailability")]
    public async Task<IActionResult> GetAllReservationOfAvailability([Required] int availabilityId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BadRequestResponse());
        }
        try
        {
            var res = await _reservationData.GetAllReservationOfAvailabilityAsync(availabilityId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response(ex.Message));
        }
    }

}