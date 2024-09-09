using api.Attributes;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models;
using api.Models.Responce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/[controller]")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly IServiceData _serviceData;

        public ServiceController(IServiceData serviceData)
        {
            _serviceData = serviceData;
        }

        [HttpGet]
        [Route("GetAllServices")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var services = await _serviceData.GetAllServicesAsync().ConfigureAwait(false);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }
    }
}
