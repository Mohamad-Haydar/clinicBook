using api.Models.Request;
using Microsoft.Extensions.Options;
using api.Helper;
using api.Models.Responce;
using Npgsql;
using System.Data;
using api.Internal.DataAccess;
using api.Data;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;
using api.BusinessLogic.DataAccess.IDataAccess;

namespace api.BusinessLogic.DataAccess;

public class ReservationData : IReservationData
{
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly ApplicationDbContext _appDbContext;
    private readonly ISqlDataAccess _sql;
    public ReservationData(ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings, ApplicationDbContext appDbContext)
    {
        _sql = sql;
        _connectionStrings = connectionStrings;
        _appDbContext = appDbContext;
    }

    public async Task CreateQueueReservationAsync(CreateQueueReservationRequest data)
    {
        try
        {
            await _sql.SaveDataAsync<CreateQueueReservationRequest>("sp_create_queue_reservation", data, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Dictionary<string, object>> GetReservationDetailsAsync(int id)
    {
        try
        {
            string[] paramsName = ["_id"];
            object[] paramsValue = [id];

            var result = await _sql.LoadDataAsync("f_get_reservation_detail", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result.First();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IQueryable<Dictionary<string, object>>> GetAllPersonalReservationsAsync(string ClientId)
    {
        try
        {
            string[] paramsName = ["client_id"];
            object[] paramsValue = [ClientId];

            var result = await _sql.LoadDataAsync("f_get_all_personal_reservations", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IQueryable<Dictionary<string, object>>> GetConcurrentBookingsAsync(int id)
    {
        try
        {
            string[] paramsName = ["client_reservation_id"];
            object[] paramsValue = [id];

            var result = await _sql.LoadDataAsync("f_get_concurrent_reservations", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<IQueryable<Dictionary<string, object>>> GetPreviousBookingsAsync(int id)
    {
        try
        {
            string[] paramsName = ["client_reservation_id"];
            object[] paramsValue = [id];

            var result = await _sql.LoadDataAsync("f_get_previous_reservations", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task DeleteSpecificReservationAsync(int ClientReservationId)
    {
        try
        {
            await _sql.SaveDataAsync("sp_delete_specific_reservation", new { client_reservation_id = ClientReservationId }, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateSpecificReservationAsync(UpdateReservationRequest model)
    {
        try
        {
            await _sql.SaveDataAsync("sp_update_specific_reservation", model, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IQueryable<Dictionary<string, object>>> GetAllReservationForTheDayAsync(int DoctorAvailabilityId)
    {
        try
        {
            string[] paramsName = ["doctor_availability_id"];
            object[] paramsValue = [DoctorAvailabilityId];

            var result = await _sql.LoadDataAsync("f_get_all_reservation_for_the_day", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task MarkCompleteReservationAsync(int ClientReservationId)
    {
        try
        {
            var ClientReservation = await _appDbContext.ClientReservations.FirstOrDefaultAsync(x => x.Id == ClientReservationId);
            if (ClientReservation == null)
            {
                throw new UserNotFoundException("This client reservation was not found");
            }
            ClientReservation.IsDone = true;
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

}