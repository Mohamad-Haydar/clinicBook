using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace api.library.DataAccess
{
    public class AuthenticationData
    {
        public async Task RegisterClientAsync(CreateUserRequest model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(new { message = "email already exists" });
            }

            using (var transaction = _identityContext.Database.BeginTransaction())
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
                    transaction.Commit();
                    return Ok(new { message = "Client created successfully. You can login to your account." });
                }
                catch (Exception) 
                {
                    throw;
                }
            }
        }

        public async Task RegisterSecretary([FromBody] CreateSecretaryRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Please enter a valid input", errors });
            }
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(new { message = "email already exists" });
            }

            using (var transaction = _identityContext.Database.BeginTransaction())
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
                    transaction.Commit();
                    return Ok(new { message = "Secretary created successfully. You can login to your account." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(new { message = ex.Message });
                }
            }
        }

        public async Task RegisterDoctor([FromBody] CreateDoctorRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Please enter a valid input", errors });
            }
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(new { message = "email already exists" });
            }

            using (var transaction = _identityContext.Database.BeginTransaction())
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
                    await _userManager.AddToRoleAsync(user, Roles.Doctor.ToString());
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
                    transaction.Commit();
                    return Ok(new { message = "Doctor created successfully. You can login to your account." });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return BadRequest(new { message = "Something went wrong. Please try again." });
                }
            }
        }

        public async Task RegisterAdmin(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Please enter a valid input", errors });
            }
            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists != null)
            {
                return BadRequest(new { message = "email already exists" });
            }
            try
            {
                var user = new UserModel { UserName = "admin", Email = email, PhoneNumber = "76612235" };
                var result = await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                return Ok(new { message = "Admin created successfully. You can login to your account." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Something went wrong. Please try again." });
            }
        }

        public async Task LoginUser([Required] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Please enter a valid input", errors });
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var accessToken = await _tokenService.GenerateAccessTokenAsync(model.Email);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                _identityContext.SaveChanges();

                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });

                return new ObjectResult(
                    new AuthenticationResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                    );
            }
            else
            {
                return BadRequest(new { message = "Wrong password" });
            }
        }

        public async Task Logout()
        {
            KeyValuePair<string, string> refreshPair = Request.Cookies.FirstOrDefault(x => x.Key == "refreshToken");
            KeyValuePair<string, string> accessPair = Request.Cookies.FirstOrDefault(x => x.Key == "accessToken");
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessPair.Value);
            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await _userManager.FindByIdAsync(userId);
            if (user.RefreshToken != refreshPair.Value)
            {
                Response.Cookies.Delete("accessToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });

                return BadRequest("Invalid client request");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            _identityContext.SaveChanges();

            Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });


            return Ok(new { message = "Log out Successfully" });
        }

    }
}
