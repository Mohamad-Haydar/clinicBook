using System.ComponentModel.DataAnnotations;
using api.Data;
using api.library.DataAccess;
using api.library.Helper;
using api.library.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api.Controllers;

// [Authorize]
[Route("/api/[controller]")]
public class MakeReservationController : Controller
{
    private readonly ApplicationDbContext _appDbContext;
    private readonly ReservationData _reservationData;

    public MakeReservationController(ApplicationDbContext appDbContext, ReservationData reservationData)
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
            return BadRequest(new {message="please enter a valid input"});
        }

        var res = await _reservationData.CreateQueueReservationAsync(model);

        if(res)
        {
            // TODO: return the date and time of his reservation
            return Ok(new {message="Reservation added successfully"});
        }
        return BadRequest(new {message="something whent wrong please check your input data"});
    }

    [HttpGet]
    [Route("GetReservationDetail")]
    public async Task<IActionResult> GetReservationDetails([Required] int id)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter a valid input"});
        }

        var res = await _reservationData.GetReservationDetailsAsync(id);

        if(res != null)
        {
            return Ok(res);
        }
        return BadRequest(new {message="something whent wrong please check your input data"});
    }

    [HttpGet]
    [Route("GetAllPersonalReservations")]
    public async Task<IActionResult> GetAllPersonalReservations([Required] string ClientId)
    {
        var res = await _reservationData.GetAllPersonalReservationsAsync(ClientId);
        if(res != null)
        {
            return Ok(res);
        }
        return BadRequest("something when wrong please check you input and try again");
    }

    [HttpDelete]
    [Route("DeleteSpecificReservation")]
    public async Task<IActionResult> DeleteSpecificReservation([Required] int clientReservationId)
    {
        if(!ModelState.IsValid)
            return BadRequest(new {message = "Please enter a valid input"});
        
        // var exists = await _appDbContext.ClientReservations.FirstOrDefaultAsync(x => x.Id == clientReservationId);
        // if(exists != null)
        // {
        //     var result = _appDbContext.ClientReservations.Remove(exists);
        //     await _appDbContext.SaveChangesAsync();
        //     return Ok(new {message="your reservation is removed successfully"});
        // }
        var result = await _reservationData.DeleteSpecificReservationAsync(clientReservationId);
        if(result)
        {
            return Ok(new {message="your reservation is removed successfully"}); 
        }
        return BadRequest(new {message = "Please check your input and try again"});
    }


}