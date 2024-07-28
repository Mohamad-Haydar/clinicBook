using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IDoctorAvailabilityData
    {
        Task DeleteAvailableDateAsync(int id);
        Task<IEnumerable<DoctorAvailabilityResponse>> GetAvailableDates(string id);
        Task OpenAvailableDateAsync(OpenAvailableDateRequest model);
        Task UpdateAvailableDateAsync(UpdateAvailableDateRequest model);
    }
}