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
                return Ok(new { message = "Backup successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to initiate backup", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("Restore")]
        public async Task<IActionResult> Restore()
        {
            try
            {
                await _backupService.CreateRestoreAsync();
                return Ok(new { message = "Restored successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to initiate backup", error = ex.Message });
            }
        }
    }
}
