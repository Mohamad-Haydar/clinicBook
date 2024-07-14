using api.Attributes;
using api.Data;
using api.library.DataAccess;
using api.library.Models.Request;
using api.library.Models.Responce;
using api.Models;
using api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[AuthorizeRoles(Roles.Admin, Roles.Secretary)]
[Route("api/[controller]")]
public class DoctorManagementController : ControllerBase
{
    private readonly IdentityAppDbContext _identityContext;
    private readonly UserManager<UserModel> _userManager;
    private readonly ApplicationDbContext _appDbContext;
    private readonly DoctorServiceData _doctorServiceData;
    public DoctorManagementController(ApplicationDbContext appDbContext, IdentityAppDbContext identityContext, UserManager<UserModel> userManager, DoctorServiceData doctorServiceData)
    {
        _appDbContext = appDbContext;
        _identityContext = identityContext;
        _userManager = userManager;
        _doctorServiceData = doctorServiceData;
    }

    [HttpPost]
    [Route("addDoctorService")]
    public async Task<IActionResult> AddDoctorService([FromBody] DoctorServiceRequest doctorService)
    {
        var res = await _doctorServiceData.AddDoctorServiceAsync(doctorService);
        if(res)
        {
            return Ok(new {message="Service added successfully"});
        }
        return BadRequest(new {message="something whent wrong please check your input data"});
    }

    [HttpPost]
    [Route("addMultipleService")]
    public async Task<IActionResult> AddMultipleService([FromBody] List<DoctorServiceRequest> doctorServices)
    {
        using (var transaction = _appDbContext.Database.BeginTransaction())
        {
            foreach (var doctorService in doctorServices)
            {
                var res = await _doctorServiceData.AddDoctorServiceAsync(doctorService);    
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
    [Route("updateDoctorServiceDuration")]
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
    [Route("deleteDoctorService")]
    //TODO: when deleting i should delete the reservation detail 
    // and check if i want to delete the client reservation
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

    [HttpDelete]
    [Route("removeDoctor")]
    public async Task<IActionResult> RemoveDoctor(string id)
    {
        var doctor = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.Id == id);
        var user = await _userManager.FindByIdAsync(id);
        if(doctor == null || user == null)
        {
            return BadRequest(new {message="Doctor not foud, enter a correct email"});
        }
        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                _appDbContext.Remove(doctor);
                await _userManager.DeleteAsync(user);
                await _identityContext.SaveChangesAsync();
                await _appDbContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "Doctor Deleted successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Something went wrong. Please try again." });
            }
        }
    }

    [HttpPatch]
    [Route("updateDoctorInfo")]
    [AuthorizeRoles(Roles.Doctor,Roles.Admin, Roles.Secretary)]
    public async Task<IActionResult> UpdateDoctorInfo([FromBody] CreateDoctorRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        var doctor = await _appDbContext.Doctors.FirstOrDefaultAsync(x => x.Email == model.Email);
        if(user == null || doctor == null)
        {
            return BadRequest(new {message="Doctor Does not exists"});
        }

        using (var transaction = _identityContext.Database.BeginTransaction())
        {
            try
            {
                string userName = model.FirstName ?? doctor.FirstName +  model.LastName ?? doctor.LastName;
                user.UserName = userName;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

                doctor.FirstName  =  model.FirstName ?? doctor.FirstName;
                doctor.LastName  =  model.LastName ?? doctor.LastName;
                doctor.PhoneNumber  =  model.PhoneNumber ?? doctor.PhoneNumber;
                doctor.Description  =  model.Description ?? doctor.Description;
                doctor.CategoryId = model.CategoryId > 0 ? model.CategoryId : doctor.CategoryId;

                await _appDbContext.SaveChangesAsync();
                await _identityContext.SaveChangesAsync();
                transaction.Commit();
                return Ok(new { message = "Doctor Data updated successfully." });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { message = "Something went wrong. Please try again." });
            }
        }
    }

    [HttpGet]
    [Route("getDoctorInfo")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorInfo(string email)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(new {message="please enter the email"});
        }

        IQueryable<DoctorInfoResponce> doctor = from d in _appDbContext.Doctors 
                    where d.Email == email
                    join c in _appDbContext.CategoryModels on d.CategoryId equals c.Id
                    select new DoctorInfoResponce {Id = d.Id,FirstName = d.FirstName,LastName = d.LastName, Email = d.Email,PhoneNumber = d.PhoneNumber,Description = d.Description, CategoryName = c.CategoryName};
        return Ok(doctor);
    }
}