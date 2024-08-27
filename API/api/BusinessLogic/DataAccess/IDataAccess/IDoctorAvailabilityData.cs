using api.Models.Request;
using api.Models.Responce;
using api.Models;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IDoctorAvailabilityData
    {
        Task DeleteAvailableDateAsync(int id);
        Task<IEnumerable<DoctorAvailabilityResponse>> GetAvailableDatesAsync(string id);
        Task OpenAvailableDateAsync(OpenAvailableDateRequest model);
        Task OpenRepeatedAvailableDateAsync(IEnumerable<OpenAvailableDateRequest> model);
        Task UpdateAvailableDateAsync(UpdateAvailableDateRequest model);
        Task<IEnumerable<DoctorAvailabilityModel>> GetDoctorAvailabilitiesOfDayAsync(DateOnly date);
        Task<IEnumerable<DoctorAvailabilityModel>> GetAllDoctorAvailabilitiesAsync(string doctorId);
    }
}