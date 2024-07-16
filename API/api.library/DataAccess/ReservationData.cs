using api.library.Models.Request;
using Microsoft.Extensions.Options;
using api.library.Helper;
using api.library.Models.Responce;
using Npgsql;
using System.Data;

namespace api.library.DataAccess;

public class ReservationData
{
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly ISqlDataAccess _sql;
    public ReservationData(ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings)
    {
        _sql = sql;
        _connectionStrings = connectionStrings;
    }

    public async Task CreateQueueReservationAsync(CreateQueueReservationRequest data)
    {
        try
        {
            await _sql.SaveDataAsync<CreateQueueReservationRequest>("sp_create_queue_reservation",data,_connectionStrings.Value.AppDbConnection);
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

            var result = await _sql.LoadDataAsync("f_get_reservation_detail",paramsName,paramsValue,_connectionStrings.Value.AppDbConnection);

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

            var result = await _sql.LoadDataAsync("f_get_all_personal_reservations",paramsName,paramsValue,_connectionStrings.Value.AppDbConnection);

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

            var result = await _sql.LoadDataAsync("f_get_concurrent_reservations",paramsName,paramsValue,_connectionStrings.Value.AppDbConnection);

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

            var result = await _sql.LoadDataAsync("f_get_previous_reservations",paramsName,paramsValue,_connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> DeleteSpecificReservationAsync(int ClientReservationId)
    {
        try
        {
            await _sql.SaveDataAsync("sp_delete_specific_reservation", new { client_reservation_id = ClientReservationId }, _connectionStrings.Value.AppDbConnection);
            return true;
        }
        catch (Exception)
        {
            return false;
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
}