using api.Data;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using api.Exceptions;
using api.BusinessLogic.DataAccess.IDataAccess;
using System.Security.Claims;
using System.Transactions;
using api.Controllers;

namespace api.BusinessLogic.DataAccess;

public class AuthenticationData : IAuthenticationData
{
    private readonly ILogger<AuthenticationData> _logger;
    private readonly IdentityAppDbContext _identityContext;
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _appContext;
    private readonly ITokenService _tokenService;

    public AuthenticationData(IdentityAppDbContext identityContext,
                              UserManager<UserModel> userManager,
                              RoleManager<IdentityRole> roleManager,
                              ApplicationDbContext appContext,
                              ITokenService tokenService,
                              ILogger<AuthenticationData> logger)
    {
        _identityContext = identityContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _appContext = appContext;
        _tokenService = tokenService;
        _logger = logger;
    }
    public async Task<AuthenticationResponse> RegisterClientAsync(CreateUserRequest model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (userExists != null)
        {
            throw new UserAlreadyExistsException();
        }

        using (var identityTransaction = await _identityContext.Database.BeginTransactionAsync().ConfigureAwait(false))
        using (var appTransaction = await _appContext.Database.BeginTransactionAsync().ConfigureAwait(false))
        {
            try
            {
                var user = new UserModel { UserName = model.FirstName + " " + model.LastName, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString()).ConfigureAwait(false);
                ClientModel client = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };

                var accessToken = await _tokenService.GenerateAccessTokenAsync(model.Email).ConfigureAwait(false);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.OldRefreshToken = refreshToken;
                user.OldRefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _appContext.Clients.AddAsync(client).ConfigureAwait(false);
                await _appContext.SaveChangesAsync().ConfigureAwait(false);
                await _identityContext.SaveChangesAsync().ConfigureAwait(false);
                await identityTransaction.CommitAsync().ConfigureAwait(false);
                await appTransaction.CommitAsync().ConfigureAwait(false);
                return new AuthenticationResponse()
                {
                    Id = user.Id,
                    UserName = model.FirstName + " " + model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Roles = [Roles.Client.ToString()]
                };
            }
            catch (Exception ex)
            {
                identityTransaction.Rollback();
                appTransaction.Rollback();
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }

    public async Task RegisterSecretaryAsync(CreateSecretaryRequest model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (userExists != null)
        {
            throw new UserAlreadyExistsException();
        }

        using (var identityTransaction = await _identityContext.Database.BeginTransactionAsync().ConfigureAwait(false))
        using (var appTransaction = await _appContext.Database.BeginTransactionAsync().ConfigureAwait(false))
        {
            try
            {
                var user = new UserModel { UserName = model.FirstName + " " + model.LastName, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                await _userManager.AddToRoleAsync(user, Roles.Secretary.ToString()).ConfigureAwait(false);
                SecretaryModel secretary = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                ClientModel client = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                await _appContext.Secretaries.AddAsync(secretary).ConfigureAwait(false);
                await _appContext.Clients.AddAsync(client).ConfigureAwait(false);
                await _appContext.SaveChangesAsync().ConfigureAwait(false);
                await _identityContext.SaveChangesAsync().ConfigureAwait(false);
                await identityTransaction.CommitAsync().ConfigureAwait(false);
                await appTransaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                identityTransaction.Rollback();
                appTransaction.Rollback();
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }

    public async Task RegisterDoctorAsync(CreateDoctorRequest model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (userExists != null)
        {
            throw new UserAlreadyExistsException();
        }

        using (var identityTransaction = await _identityContext.Database.BeginTransactionAsync().ConfigureAwait(false))
        using (var appTransaction = await _appContext.Database.BeginTransactionAsync().ConfigureAwait(false))
        {
            try
            {
                string userName = model.FirstName + " " + model.LastName;
                var user = new UserModel
                {
                    UserName = userName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                await _userManager.AddToRoleAsync(user, Roles.Doctor.ToString()).ConfigureAwait(false);
                DoctorModel doctor = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Image = model.Image,
                };
                await _appContext.Doctors.AddAsync(doctor).ConfigureAwait(false);
                await _appContext.SaveChangesAsync().ConfigureAwait(false);
                await _identityContext.SaveChangesAsync().ConfigureAwait(false);

                await appTransaction.CommitAsync().ConfigureAwait(false);
                await identityTransaction.CommitAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await appTransaction.RollbackAsync().ConfigureAwait(false);
                await identityTransaction.RollbackAsync().ConfigureAwait(false);
                _logger.LogError(ex.Message);
                throw new BusinessException();
            }
        }
    }

    public async Task RegisterAdminAsync(string email, string password)
    {
        var userExists = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (userExists != null)
        {
            throw new UserAlreadyExistsException("This email already exists");
        }
        try
        {
            var user = new UserModel { UserName = "admin", Email = email, PhoneNumber = "76612235" };
            var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);
            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString()).ConfigureAwait(false);
            await _appContext.SaveChangesAsync().ConfigureAwait(false);
            await _identityContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException("Something went wrong. Please try again.");
        }
    }

    public async Task<AuthenticationResponse> LoginUserAsync(LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false) ?? throw new UserNotFoundException();
        try
        {
            if (await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var accessToken = await _tokenService.GenerateAccessTokenAsync(model.Email).ConfigureAwait(false);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.OldRefreshToken = refreshToken;
                user.OldRefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                _identityContext.SaveChanges();

                return new AuthenticationResponse
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = model.Email,
                    PhoneNumber = user.PhoneNumber,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    Roles = roles
                };
            }
            else
            {
                throw new WrongPasswordException();
            }
        }
        catch (WrongPasswordException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BusinessException();
        }
    }

    public async Task LogoutAsync(string refreshToken, string accessToken)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if(user.RefreshToken != refreshToken)
            {
                throw new UserNotFoundException("Invalid client request");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            _identityContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

    }

}

