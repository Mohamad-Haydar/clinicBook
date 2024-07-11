using api.Data;
using api.library.DataAccess;
using api.library.Helper;
using api.library.Internal.DataAccess;
using api.library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api.Controllers;

// [Authorize]
public class DoctorManagementController : ControllerBase
{
    private readonly ApplicationDbContext _appDbContext;
    private readonly ISqlDataAccess _sql;
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    public DoctorManagementController(ApplicationDbContext appDbContext, ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings)
    {
        _appDbContext = appDbContext;
        _sql = sql;
        _connectionStrings = connectionStrings;
    }
    
    [HttpPost]
    [Route("add_doctor_service")]
    public async Task<IActionResult> AddDoctorService([FromBody] DoctorServiceRequest doctorService)
    {
        DoctorServiceData doctorServiceData = new(_sql,_connectionStrings);
        var res = await doctorServiceData.AddDoctorServiceAsync(doctorService);
        if(res)
        {
            return Ok(new {message="Service added successfully"});
        }
        return BadRequest(new {message="something whent wrong please check your input data"});
    }

    [HttpPost]
    [Route("add_multiple_service")]
    public async Task<IActionResult> AddMultipleService([FromBody] List<DoctorServiceRequest> doctorServices)
    {
        DoctorServiceData doctorServiceData = new DoctorServiceData(_sql,_connectionStrings);
        using (var transaction = _appDbContext.Database.BeginTransaction())
        {
            foreach (var doctorService in doctorServices)
            {
                var res = await doctorServiceData.AddDoctorServiceAsync(doctorService);    
                if(!res)
                {
                    transaction.Rollback();
                    return BadRequest(new {message="something whent wrong please check your input data"});
                }
            }
            return Ok(new {message="All Services added successfully"});
        }
    }

    [HttpPatch]
    [Route("update_doctor_service_duration")]
    public async Task<IActionResult> UpdateDoctorServiceDuration(int id, int duration)
    {
        var service = await _appDbContext.DoctorServiceModels.FirstOrDefaultAsync(x => x.Id == id);
        if(service == null)
        {
            return BadRequest(new {message="something whent wrong please check your input data"});
        }
        service.Duration = duration;
        await _appDbContext.SaveChangesAsync();
        return Ok(new {message="Service Duration updated successfully"});
    }

    [HttpDelete]
    [Route("delete_doctor_service")]
    public async Task<IActionResult> DeleteDoctorService(int id)
    {
        var service = await _appDbContext.DoctorServiceModels.FirstOrDefaultAsync(x => x.Id == id);
        if(service == null)
        {
            return BadRequest(new {message="something whent wrong please check your input data"});
        }
        var res = _appDbContext.DoctorServiceModels.Remove(service);
        await _appDbContext.SaveChangesAsync();
        return Ok(new {message="Service Deleted successfully"});
    }

}