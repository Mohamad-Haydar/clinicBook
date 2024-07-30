
namespace api.Services
{
    public interface IBackupService
    {
        Task CreateBackupAsync();
        Task CreateRestoreAsync();
    }
}