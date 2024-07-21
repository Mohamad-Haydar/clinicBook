using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IDoctorManagementData
    {
        Task AddDoctorServiceAsync(DoctorServiceRequest data);
        Task AddMultipleServiceAsync(List<DoctorServiceRequest> doctorServices);
        Task DeleteDoctorServiceAsync(int id);
        Task<IQueryable<DoctorInfoResponce>> GetDoctorInfoAsync(string email);
        Task RemoveDoctorAsync(string id);
        Task UpdateDoctorInfoAsync(CreateDoctorRequest model);
        Task UpdateDoctorServiceDurationAsync(int id, int duration);
    }
}