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
    private readonly IMessageProvider _messageProvider;
    public SecretaryData(ApplicationDbContext applicationDbContext, ILogger<SecretaryData> logger, IMessageProvider messageProvider)
    {
        _appDbContext = applicationDbContext;
        _logger = logger;
        _messageProvider = messageProvider;
    }

    public async Task<Result<SecretaryModel>> GetSecretariebyEmailAsync(string email)
    {
        try
        {
            var secretarie = await _appDbContext.Secretaries.FirstOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);
            if(secretarie == null) return Result<SecretaryModel>.Failure(_messageProvider.GetMessage("userNotFound"));
            return Result<SecretaryModel>.Success(secretarie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }
}