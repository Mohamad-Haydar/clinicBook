using api.Exceptions;
using api.Models.Responce;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("/api/[controller]")]
    public class BackupController : Controller
    {
        private readonly IBackupService _backupService;

        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        [HttpPost]
        [Route("Backup")]
        public IActionResult Backup()
        {
            try
            {
                _backupService.CreateBackupAsync();
                return Ok(new Response("لقد تم انشاء نسخة احتياط بنجاح"));
            }
            catch (Exception)
            {
                return BadRequest( new BusinessException());
            }
        }

        [HttpPost]
        [Route("Restore")]
        public async Task<IActionResult> Restore()
        {
            try
            {
                await _backupService.CreateRestoreAsync();
                return Ok(new Response("لقد تم استعادة المعلومات بنجاح"));
            }
            catch (Exception)
            {
                return BadRequest(new BusinessException());
            }
        }
    }
}
