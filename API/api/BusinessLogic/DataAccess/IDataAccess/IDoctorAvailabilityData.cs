using api.Models.Request;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IDoctorAvailabilityData
    {
        Task DeleteAvailableDateAsync(int id);
        object GetAvailableDates(string id);
        Task OpenAvailableDateAsync(OpenAvailableDateRequest model);
        Task UpdateAvailableDateAsync(UpdateAvailableDateRequest model);
    }
}