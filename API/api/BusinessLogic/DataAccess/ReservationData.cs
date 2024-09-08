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
using api.Models;
using System.Text.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace api.BusinessLogic.DataAccess;

public class ReservationData : IReservationData
{
    private readonly ILogger<ReservationData> _logger;
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly ApplicationDbContext _appDbContext;
    private readonly ISqlDataAccess _sql;
    public ReservationData(ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings, ApplicationDbContext appDbContext, ILogger<ReservationData> logger)
    {
        _sql = sql;
        _connectionStrings = connectionStrings;
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task CreateQueueReservationAsync(CreateQueueReservationRequest data)
    {
        try
        {
            await _sql.SaveDataAsync<CreateQueueReservationRequest>("sp_create_queue_reservation", data, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<List<Dictionary<string, object>>> GetAllPersonalReservationsAsync(string ClientId)
    {
        try
        {
            string[] paramsName = ["client_id"];
            object[] paramsValue = [ClientId];

            var result = await _sql.LoadDataAsync("f_get_all_personal_reservations", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<List<Dictionary<string, object>>> GetConcurrentBookingsAsync(int id)
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
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task<List<Dictionary<string, object>>> GetPreviousBookingsAsync(int id)
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
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task DeleteSpecificReservationAsync(int ClientReservationId, string userData, string accessToken)
    {
        // new Claim(ClaimTypes.NameIdentifier, user.Id),
        var user = JsonSerializer.Deserialize<CookieUserModel>(userData);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        if(userId != user.id)
        {
            throw new BusinessException();
        }
        try
        {
            await _sql.SaveDataAsync("sp_delete_specific_reservation", new { client_reservation_id = ClientReservationId }, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task UpdateSpecificReservationAsync(UpdateReservationRequest model)
    {
        try
        {
            await _sql.SaveDataAsync("sp_update_specific_reservation", model, _connectionStrings.Value.AppDbConnection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task<List<Dictionary<string, object>>> GetAllReservationForTheDayAsync(int DoctorAvailabilityId)
    {
        try
        {
            string[] paramsName = ["doctor_availability_id"];
            object[] paramsValue = [DoctorAvailabilityId];

            var result = await _sql.LoadDataAsync("f_get_all_reservation_for_the_day", paramsName, paramsValue, _connectionStrings.Value.AppDbConnection);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task MarkCompleteReservationAsync(int ClientReservationId)
    {
        try
        {
            var ClientReservation = await _appDbContext.ClientReservations.FirstOrDefaultAsync(x => x.Id == ClientReservationId);
            if (ClientReservation == null)
            {
                throw new UserNotFoundException();
            }
            ClientReservation.IsDone = true;
            await _appDbContext.SaveChangesAsync();
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task<IEnumerable<ReservationDetailResponce>> GetAllReservationOfAvailabilityAsync(int availabilityId)
    {
        try
        {
            var res = from cr in _appDbContext.ClientReservations
                      where cr.DoctorAvailabilityId == availabilityId
                      join c in _appDbContext.Clients on cr.ClientId equals c.Id
                      orderby cr.StartTime
                      select new ReservationDetailResponce { id = cr.Id, startTime = cr.StartTime, endTime = cr.EndTime, isDone = cr.IsDone , clientName = c.FirstName + " " + c.LastName, details = cr.Details };
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

}