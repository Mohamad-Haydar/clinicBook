using api.Data;
using api.Exceptions;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using api.Models.Responce;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace api.BusinessLogic.DataAccess;

public class SecretaryData
{

    private readonly ApplicationDbContext _appDbContext;
    private readonly ILogger<SecretaryData> _logger;
    public SecretaryData(ApplicationDbContext applicationDbContext, ILogger<SecretaryData> logger)
    {
        _appDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<SecretaryModel> GetSecretariebyEmailAsync(string email)
    {
        try
        {
            var secretarie = await _appDbContext.Secretaries.FirstOrDefaultAsync(x => x.Email == email) ?? throw new UserNotFoundException();
            return secretarie;
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
}