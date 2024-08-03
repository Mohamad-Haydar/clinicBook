using api.Data;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using api.Exceptions;
using api.BusinessLogic.DataAccess.IDataAccess;
using System.Security.Claims;
using System.Transactions;

namespace api.BusinessLogic.DataAccess;

public class AuthenticationData : IAuthenticationData
{

    private readonly IdentityAppDbContext _identityContext;
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _appContext;
    private readonly ITokenService _tokenService;

    public AuthenticationData(IdentityAppDbContext identityContext,
                              UserManager<UserModel> userManager,
                              RoleManager<IdentityRole> roleManager,
                              ApplicationDbContext appContext,
                              ITokenService tokenService)
    {
        _identityContext = identityContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _appContext = appContext;
        _tokenService = tokenService;
    }
    public async Task RegisterClientAsync(CreateUserRequest model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            throw new UserExistsException("This email already exists");
        }

        using (var identityTransaction = await _identityContext.Database.BeginTransactionAsync())
        using (var appTransaction = await _appContext.Database.BeginTransactionAsync())
        {
            try
            {
                var user = new UserModel { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, Roles.Client.ToString());
                ClientModel client = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                await _appContext.Clients.AddAsync(client);
                await _appContext.SaveChangesAsync();
                await _identityContext.SaveChangesAsync();
                await identityTransaction.CommitAsync();
                await appTransaction.CommitAsync();
            }
            catch
            {
                identityTransaction.Rollback();
                appTransaction.Rollback();
                throw new BusinessException("An error occurred while registering the client");
            }
        }
    }

    public async Task RegisterSecretaryAsync(CreateSecretaryRequest model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            throw new UserExistsException("This email already exists");
        }

        using (var identityTransaction = await _identityContext.Database.BeginTransactionAsync())
        using (var appTransaction = await _appContext.Database.BeginTransactionAsync())
        {
            try
            {
                var user = new UserModel { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, Roles.Secretary.ToString());
                SecretaryModel secretary = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                };
                await _appContext.Secretaries.AddAsync(secretary);
                await _appContext.SaveChangesAsync();
                await _identityContext.SaveChangesAsync();
                await identityTransaction.CommitAsync();
                await appTransaction.RollbackAsync();
            }
            catch (Exception)
            {
                identityTransaction.Rollback();
                appTransaction.Rollback();
                throw new BusinessException("An error occurred while registering the client");
            }
        }
    }

    public async Task RegisterDoctorAsync(CreateDoctorRequest model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            throw new UserExistsException("This email already exists");
        }

        using (var identityTransaction = await _identityContext.Database.BeginTransactionAsync())
        using (var appTransaction = await _appContext.Database.BeginTransactionAsync())
        {
            try
            {
                string userName = model.FirstName + model.LastName;
                var user = new UserModel
                {
                    UserName = userName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    RefreshToken = "",
                    RefreshTokenExpiryTime = DateTime.MinValue
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                var added = await _userManager.AddToRoleAsync(user, Roles.Doctor.ToString());
                DoctorModel doctor = new()
                {
                    Id = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Description = model.Description,
                    CategoryId = model.CategoryId
                };
                await _appContext.Doctors.AddAsync(doctor);
                await _appContext.SaveChangesAsync();
                await _identityContext.SaveChangesAsync();

                await appTransaction.CommitAsync();
                await identityTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await appTransaction.RollbackAsync();
                await identityTransaction.RollbackAsync();
                throw new BusinessException("An error occurred while registering the client");
            }
        }
    }

    public async Task RegisterAdminAsync(string email, string password)
    {
        var userExists = await _userManager.FindByEmailAsync(email);
        if (userExists != null)
        {
            throw new UserExistsException("This email already exists");
        }
        try
        {
            var user = new UserModel { UserName = "admin", Email = email, PhoneNumber = "76612235" };
            var result = await _userManager.CreateAsync(user, password);
            await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            await _appContext.SaveChangesAsync();
            await _identityContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new BusinessException("Something went wrong. Please try again.");
        }
    }

    public async Task<AuthenticationResponse> LoginUserAsync(LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            throw new UserExistsException("User Not found Please enter a valid input");
        }
        if (await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var accessToken = await _tokenService.GenerateAccessTokenAsync(model.Email);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _identityContext.SaveChanges();

            return new AuthenticationResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = model.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        else
        {
            throw new WrongPasswordException("Wrong password, please enter a valid password.");
        }
    }

    public async Task LogoutAsync(string refreshToken, string accessToken)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);
            if(user.RefreshToken != refreshToken)
            {
                throw new NotFoundException("Invalid client request");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            _identityContext.SaveChanges();
        }
        catch (System.Exception)
        {   
            throw;
        }

    }

}

