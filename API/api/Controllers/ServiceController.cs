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

        [HttpPost]
        [Route("CreateService")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateService(string serviceName)
        {
            try
            {
                await _serviceData.CreateServiceAsync(serviceName).ConfigureAwait(false);
                return Ok(new Response("تم انشاء الخدمة بنجاح."));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }

        [HttpPatch]
        [Route("UpdateService")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateService([FromBody] ServiceModel model)
        {
            try
            {
                await _serviceData.UpdateServiceAsync(model).ConfigureAwait(false);
                return Ok(new Response());
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }

        [HttpDelete]
        [Route("DeleteService")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                await _serviceData.DeleteServiceAsync(id).ConfigureAwait(false);
                return Ok(new Response());
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(ex.Message));
            }
        }
    }
}
