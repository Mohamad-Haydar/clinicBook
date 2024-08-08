using api.Models.Request;
using api.Models.Responce;

namespace api.BusinessLogic.DataAccess.IDataAccess
{
    public interface IReservationData
    {
        Task CreateQueueReservationAsync(CreateQueueReservationRequest data);
        Task DeleteSpecificReservationAsync(int ClientReservationId);
        Task<IQueryable<Dictionary<string, object>>> GetAllPersonalReservationsAsync(string ClientId);
        Task<IQueryable<Dictionary<string, object>>> GetAllReservationForTheDayAsync(int DoctorAvailabilityId);
        Task<IQueryable<Dictionary<string, object>>> GetConcurrentBookingsAsync(int id);
        Task<IQueryable<Dictionary<string, object>>> GetPreviousBookingsAsync(int id);
        Task<Dictionary<string, object>> GetReservationDetailsAsync(int id);
        Task MarkCompleteReservationAsync(int ClientReservationId);
        Task UpdateSpecificReservationAsync(UpdateReservationRequest model);
    }
}