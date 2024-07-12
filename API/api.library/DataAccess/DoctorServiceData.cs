using api.library.Models.Request;
using api.library.Internal.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using api.library.Helper;

namespace api.library.DataAccess;

public class DoctorServiceData
{
    private readonly IOptions<ConnectionStrings> _connectionStrings;
    private readonly ISqlDataAccess _sql;
    public DoctorServiceData(ISqlDataAccess sql, IOptions<ConnectionStrings> connectionStrings)
    {
        _sql = sql;
        _connectionStrings = connectionStrings;
    }

    public async Task<bool> AddDoctorServiceAsync(DoctorServiceRequest data)
    {
        try
        {
            await _sql.SaveDataAsync<DoctorServiceRequest>("sp_insert_doctor_service",data,_connectionStrings.Value.AppDbConnection);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}