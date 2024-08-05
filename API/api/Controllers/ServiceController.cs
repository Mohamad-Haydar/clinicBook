using api.BusinessLogic.DataAccess.IDataAccess;
using api.Models.Responce;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/[controller]")]
    public class ServiceController : Controller
    {
        private readonly IServiceData _serviceData;

        public ServiceController(IServiceData serviceData)
        {
            _serviceData = serviceData;
        }

        [HttpGet]
        [Route("GetAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var services = await _serviceData.GetAllServicesAsync();
                return Ok(services);
            }
            catch (Exception)
            {
                return BadRequest(new Response("Something whent wrong, please try again"));
            }
        }
    }
}
