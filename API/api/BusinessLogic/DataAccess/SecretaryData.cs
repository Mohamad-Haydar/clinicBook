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
    public SecretaryData(ApplicationDbContext applicationDbContext)
    {
        _appDbContext = applicationDbContext;
    }

    public async Task<SecretaryModel> GetSecretariebyEmailAsync(string email)
    {
        try
        {
            var secretarie = await _appDbContext.Secretaries.FirstOrDefaultAsync(x => x.Email == email) ?? throw new UserNotFoundException("Secretary not found, please enter a correct email");
            return secretarie;
        }
        catch (Exception)
        {   
            throw;
        }
    }
}