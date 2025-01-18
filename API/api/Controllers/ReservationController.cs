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
using System.Text.Json;

namespace api.Controllers;

[Authorize]
[Route("/api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationData _reservationData;
    private readonly IMessageProvider _messageProvider;

    public ReservationController(IReservationData reservationData, IMessageProvider messageProvider)
    {
        _reservationData = reservationData;
        _messageProvider = messageProvider;
    }


    [HttpPost]
    [Route("CreateQueueReservation")]
    public async Task<IActionResult> CreateQueueReservation([FromBody] CreateQueueReservationRequest model)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            await _reservationData.CreateQueueReservationAsync(model).ConfigureAwait(false);
            return Ok(Result.Success(_messageProvider.GetMessage("createQueueReservationSuccess")));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetReservationDetail")]
    public async Task<IActionResult> GetReservationDetails([Required] int id)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));
        
        try
        {
            var res = await _reservationData.GetReservationDetailsAsync(id).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetAllPersonalReservations")]
    public async Task<IActionResult> GetAllPersonalReservations([Required] string ClientId)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var res = await _reservationData.GetAllPersonalReservationsAsync(ClientId).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetConcurrentBookings")]
    public async Task<IActionResult> GetConcurrentBookings([Required] int id)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var result = await _reservationData.GetConcurrentBookingsAsync(id).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetPreviousBookings")]
    public async Task<IActionResult> GetPreviousBookings([Required] int id)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var result = await _reservationData.GetPreviousBookingsAsync(id).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }

    }

    [HttpDelete]
    [Route("DeleteSpecificReservation")]
    public async Task<IActionResult> DeleteSpecificReservation([Required] int clientReservationId)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var userData = Request.Cookies["userData"];
            var accessToken = Request.Cookies["accessToken"];
            if(userData == null || accessToken == null)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("pleaseLoginError")));
            }
            var res = await _reservationData.DeleteSpecificReservationAsync(clientReservationId, userData, accessToken).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }


    }
    
    [HttpPatch]
    [Route("UpdateSpecificReservation")]
    public async Task<IActionResult> UpdateSpecificReservation([FromBody] UpdateReservationRequest model)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            await _reservationData.UpdateSpecificReservationAsync(model).ConfigureAwait(false);
            return Ok(Result.Success(_messageProvider.GetMessage("updateSpecificReservation")));
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetAllReservationForTheDay")]
    public async Task<IActionResult> GetAllReservationForTheDay([Required] int DoctorAvailabilityId)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var result = await _reservationData.GetAllReservationForTheDayAsync(DoctorAvailabilityId).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpPost]
    [Route("MarkCompleteReservation")]
    public async Task<IActionResult> MarkCompleteReservation([Required] int ClientReservationId)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var res = await _reservationData.MarkCompleteReservationAsync(ClientReservationId).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

    [HttpGet]
    [Route("GetAllReservationOfAvailability")]
    public async Task<IActionResult> GetAllReservationOfAvailability([Required] int availabilityId)
    {
        if(!ModelState.IsValid) return BadRequest(Result.Failure(_messageProvider.GetMessage("wrongInput")));

        try
        {
            var res = await _reservationData.GetAllReservationOfAvailabilityAsync(availabilityId).ConfigureAwait(false);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
        }
    }

}