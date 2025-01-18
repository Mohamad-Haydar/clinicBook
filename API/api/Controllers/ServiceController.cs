using api.Attributes;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Helper;
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
        private readonly IMessageProvider _messageProvider;

        public ServiceController(IServiceData serviceData, IMessageProvider messageProvider)
        {
            _serviceData = serviceData;
            _messageProvider = messageProvider;
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
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }

        [HttpPost]
        [Route("CreateService")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateService(string serviceName)
        {
            try
            {
                var res = await _serviceData.CreateServiceAsync(serviceName).ConfigureAwait(false);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }

        [HttpPatch]
        [Route("UpdateService")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateService([FromBody] ServiceModel model)
        {
            try
            {
                var res = await _serviceData.UpdateServiceAsync(model).ConfigureAwait(false);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }

        [HttpDelete]
        [Route("DeleteService")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var res = await _serviceData.DeleteServiceAsync(id).ConfigureAwait(false);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                return BadRequest(Result.Failure(_messageProvider.GetMessage("error")));
            }
        }
    }
}
