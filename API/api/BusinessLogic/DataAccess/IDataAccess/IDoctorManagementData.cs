using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Mvc;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IDoctorManagementData
    {
        Task AddDoctorServiceAsync(DoctorServiceRequest data);
        Task AddMultipleServiceAsync(List<DoctorServiceRequest> doctorServices);
        Task<Result> DeleteDoctorServiceAsync(int id);
        Task<Result<DoctorInfoResponse>> GetDoctorByEmailAsync(string email);
        Task<Result<DoctorInfoResponse>> GetDoctorByIdAsync(string id);
        Task<IEnumerable<DoctorInfoResponse>> GetAllDoctorsAsync();
        Task<IEnumerable<DoctorInfoResponse>> GetDoctorsByCategoryAsync(int CategoryId);
        Task<Result> RemoveDoctorAsync(string id);
        Task<Result> UpdateDoctorInfoAsync(UpdateDoctorRequest model);
        Task<Result> UpdateDoctorServiceDurationAsync(int id, int duration);
        Task<string> UploadImageAsync(IFormFile file);
    }
}