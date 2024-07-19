using System.ComponentModel.DataAnnotations;
using api.Data;
using api.library.DataAccess;
using api.library.Helper;
using api.library.Models.Request;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

//[Authorize]
[Route("/api/[controller]")]
public class ReservationController : Controller
{
    private readonly ApplicationDbContext _appDbContext;
    private readonly ReservationData _reservationData;

    public ReservationController(ApplicationDbContext appDbContext, ReservationData reservationData)
    {
        _appDbContext = appDbContext;
        _reservationData = reservationData;
    }


    [HttpPost]
    [Route("CreateQueueReservation")]
    public async Task<IActionResult> CreateQueueReservation([FromBody] CreateQueueReservationRequest model)
    {
        if(!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Please enter a valid input", errors });
        }

        try
        {
            await _reservationData.CreateQueueReservationAsync(model);
            return Ok(new {message="Reservation added successfully"});
        }
        catch (Exception)
        {
            return BadRequest(new {message= "An error occurred while processing your request." });
        }
    }   

    [HttpGet]
    [Route("GetReservationDetail")]
    public async Task<IActionResult> GetReservationDetails([Required] int id)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter a valid input"});
        }
        try
        {
            var res = await _reservationData.GetReservationDetailsAsync(id);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new {message="Something whent wrong please try again."});
        }
    }

    [HttpGet]
    [Route("GetAllPersonalReservations")]
    public async Task<IActionResult> GetAllPersonalReservations([Required] string ClientId)
    {
         if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter a valid input"});
        }
        try
        {
            var res = await _reservationData.GetAllPersonalReservationsAsync(ClientId);
            return Ok(res);
        }
        catch (Exception)
        {
            return BadRequest(new { message="something when wrong please check you input and try again"});
        }
    }

    [HttpGet]
    [Route("GetConcurrentBookings")]
    public async Task<IActionResult> GetConcurrentBookings([Required] int ClientReservationId)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter valid input"});
        }
        try
        {
            var result = await _reservationData.GetConcurrentBookingsAsync(ClientReservationId);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new {message="something when wrong please check you input and try again"});
        }
    }

    [HttpGet]
    [Route("GetPreviousBookings")]
    public async Task<IActionResult> GetPreviousBookings([Required] int ClientReservationId)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter valid input"});
        }
        try
        {
            var result = await _reservationData.GetPreviousBookingsAsync(ClientReservationId);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new {message="something when wrong please check you input and try again"});
        }

    }

    [HttpDelete]
    [Route("DeleteSpecificReservation")]
    public async Task<IActionResult> DeleteSpecificReservation([Required] int clientReservationId)
    {
        if(!ModelState.IsValid)
            return BadRequest(new {message = "Please enter a valid input"});

        try
        {
            await _reservationData.DeleteSpecificReservationAsync(clientReservationId);
            return Ok(new {message="your reservation is removed successfully"}); 
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        
    }

    [HttpPatch]
    [Route("UpdatespecificReservation")]
    public async Task<IActionResult> UpdatespecificReservation([Required] UpdateReservationRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "please enter valid data" });
        }
        try
        {
            await _reservationData.UpdateSpecificReservationAsync(model);
            return Ok(new { message = "your reservation is Updated successfully" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Please check your input and try again" });
        }
    }

    [HttpGet]
    [Route("GetAllReservationForTheDay")] 
    public async Task<IActionResult> GetAllReservationForTheDay([Required] int DoctorAvailabilityId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "please enter valid input" });
        }
        try
        {
            var result = await _reservationData.GetAllReservationForTheDayAsync(DoctorAvailabilityId);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "something when wrong please check you input and try again" });
        }
    }

    [HttpPost]
    [Route("MarkCompleteReservation")]
    public async Task<IActionResult> MarkCompleteReservation([Required] int ClientReservationId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "please enter valid input" });
        }
        try
        {
            var ClientReservation = await _appDbContext.ClientReservations.FirstOrDefaultAsync(x => x.Id == ClientReservationId);
            if(ClientReservation == null)
            {
                return NotFound(new {message="This client reservation was not found"});
            }
            ClientReservation.IsDone = true;
            await _appDbContext.SaveChangesAsync();
            return Ok(new {message = "reservation marked as finished"});
        }
        catch (Exception)
        {
            return BadRequest(new { message = "something when wrong please check you input and try again" });
        }
    }
}