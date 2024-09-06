using api.Models;
using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IReservationData
    {
        Task CreateQueueReservationAsync(CreateQueueReservationRequest data);
        Task DeleteSpecificReservationAsync(int ClientReservationId, string userdata, string accessToken);
        Task<List<Dictionary<string, object>>> GetAllPersonalReservationsAsync(string ClientId);
        Task<List<Dictionary<string, object>>> GetAllReservationForTheDayAsync(int DoctorAvailabilityId);
        Task<List<Dictionary<string, object>>> GetConcurrentBookingsAsync(int id);
        Task<List<Dictionary<string, object>>> GetPreviousBookingsAsync(int id);
        Task<Dictionary<string, object>> GetReservationDetailsAsync(int id);
        Task MarkCompleteReservationAsync(int ClientReservationId);
        Task UpdateSpecificReservationAsync(UpdateReservationRequest model);
        Task<IEnumerable<ReservationDetailResponce>> GetAllReservationOfAvailabilityAsync(int availabilityId);
    }
}