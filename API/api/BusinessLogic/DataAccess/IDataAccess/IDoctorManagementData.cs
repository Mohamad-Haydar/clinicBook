using api.Models;
using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IDoctorManagementData
    {
        Task AddDoctorServiceAsync(DoctorServiceRequest data);
        Task AddMultipleServiceAsync(List<DoctorServiceRequest> doctorServices);
        Task DeleteDoctorServiceAsync(int id);
        Task<DoctorInfoResponse> GetDoctorByEmailAsync(string email);
        Task<DoctorInfoResponse> GetDoctorByIdAsync(string id);
        Task<IEnumerable<DoctorInfoResponse>> GetAllDoctorsAsync();
        Task<IEnumerable<DoctorInfoResponse>> GetDoctorsByCategoryAsync(int CategoryId);
        Task RemoveDoctorAsync(string id);
        Task UpdateDoctorInfoAsync(UpdateDoctorRequest model);
        Task UpdateDoctorServiceDurationAsync(int id, int duration);
    }
}