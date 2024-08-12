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
            return BadRequest(new Response("Please enter a valid input"));
        }

        try
        {
            await _reservationData.CreateQueueReservationAsync(model);
            return Ok(new Response("Reservation added successfully"));
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
            return BadRequest(new Response("please enter a valid input" ));
        }
        try
        {
            var res = await _reservationData.GetReservationDetailsAsync(id);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new Response("Something whent wrong please try again."));
        }
    }

    [HttpGet]
    [Route("GetAllPersonalReservations")]
    public async Task<IActionResult> GetAllPersonalReservations([Required] string ClientId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "please enter a valid input" });
        }
        try
        {
            var res = await _reservationData.GetAllPersonalReservationsAsync(ClientId);
            return Ok(res);
        }
        catch (Exception)
        {
            return BadRequest(new Response("something when wrong please check you input and try again"));
        }
    }

    [HttpGet]
    [Route("GetConcurrentBookings")]
    public async Task<IActionResult> GetConcurrentBookings([Required] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response("please enter valid input"));
        }
        try
        {
            var result = await _reservationData.GetConcurrentBookingsAsync(id);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new Response("something when wrong please check you input and try again"));
        }
    }

    [HttpGet]
    [Route("GetPreviousBookings")]
    public async Task<IActionResult> GetPreviousBookings([Required] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response("please enter valid input"));
        }
        try
        {
            var result = await _reservationData.GetPreviousBookingsAsync(id);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new Response("something when wrong please check you input and try again"));
        }

    }

    [HttpDelete]
    [Route("DeleteSpecificReservation")]
    public async Task<IActionResult> DeleteSpecificReservation([Required] int clientReservationId)
    {
        if (!ModelState.IsValid)
            return BadRequest(new Response("Please enter a valid input"));

        try
        {
            await _reservationData.DeleteSpecificReservationAsync(clientReservationId);
            return Ok(new Response("your reservation is removed successfully"));
        }
        catch (Exception)
        {
            return BadRequest(new Response("something when wrong please check you input and try again"));
            }


    }
    
    [HttpPatch]
    [Route("UpdateSpecificReservation")]
    public async Task<IActionResult> UpdateSpecificReservation([FromBody] UpdateReservationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response("please enter valid data"));
        }
        try
        {
            await _reservationData.UpdateSpecificReservationAsync(model);
            return Ok(new Response("your reservation is Updated successfully"));
        }
        catch (Exception)
        {
            return BadRequest(new Response("Please check your input and try again"));
        }
    }

    [HttpGet]
    [Route("GetAllReservationForTheDay")]
    public async Task<IActionResult> GetAllReservationForTheDay([Required] int DoctorAvailabilityId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response("please enter valid input"));
        }
        try
        {
            var result = await _reservationData.GetAllReservationForTheDayAsync(DoctorAvailabilityId);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new Response("something when wrong please check you input and try again"));
        }
    }

    [HttpPost]
    [Route("MarkCompleteReservation")]
    public async Task<IActionResult> MarkCompleteReservation([Required] int ClientReservationId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new Response("please enter valid input"));
        }
        try
        {
            await _reservationData.MarkCompleteReservationAsync(ClientReservationId);
            return Ok(new { message = "reservation marked as finished" });
        }
        catch (Exception)
        {
            return BadRequest(new Response("something when wrong please check you input and try again"));
        }
    }
}