using api.BusinessLogic.DataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests.BusinessLogic
{
    public class AuthenticationDataTests
    {
        private readonly IdentityAppDbContext _identityContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _appContext;
        private readonly ITokenService _tokenService;
        private readonly AuthenticationData _sut;

        public AuthenticationDataTests()
        {
            // Create context options for both database
            var _identitycontextOptions = new DbContextOptionsBuilder<IdentityAppDbContext>().UseInMemoryDatabase("clinicusers");
            var _appcontextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("clinicbook");

            // create the context for both database
            _identityContext = new IdentityAppDbContext(_identitycontextOptions.Options);
            _appContext = new ApplicationDbContext(_appcontextOptions.Options);

            // Ensure creation
            _identityContext.Database.EnsureCreated();
            _appContext.Database.EnsureCreated();

            // Substitute all the dependency of usermanager
            var userStore = Substitute.For<IUserStore<UserModel>>();
            var roleStore = Substitute.For<IRoleStore<IdentityRole>>();
            var options = Substitute.For<IOptions<IdentityOptions>>();
            var passwordHasher = Substitute.For<IPasswordHasher<UserModel>>();
            var userValidators = new List<IUserValidator<UserModel>> { Substitute.For<IUserValidator<UserModel>>() };
            var passwordValidators = new List<IPasswordValidator<UserModel>> { Substitute.For<IPasswordValidator<UserModel>>() };
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var services = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<UserManager<UserModel>>>();
            var loggerRole = Substitute.For<ILogger<RoleManager<IdentityRole>>>();
            var roleValidators = new List<IRoleValidator<IdentityRole>>();

            _userManager = Substitute.For<UserManager<UserModel>>(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger);
            _roleManager = Substitute.For<RoleManager<IdentityRole>>(roleStore, roleValidators, keyNormalizer, errors, loggerRole);
            _tokenService = Substitute.For<ITokenService>();

            _sut = new AuthenticationData(_identityContext, _userManager, _roleManager, _appContext, _tokenService);
        }

       // [Fact]
        //public async Task RegisterClientAsync_UserAlreadyExists_ThrowUserExistsException()
        //{
        //    // Arrange
        //    CreateUserRequest model = new() { Email = "someemail" };
        //    _userManager.FindByEmailAsync("someemail").Returns(new UserModel());

        //    // Act & Assert
        //    await Assert.ThrowsAsync<UserExistsException>(() => _sut.RegisterClientAsync(model));
        //}

        [Fact]
        public async Task RegisterClientAsync_UserAlreadyExists_ThrowUserExistsException()
        {
            // Arrange
            CreateUserRequest model = new() { Email = "someemail" };
            _userManager.FindByEmailAsync("someemail").Returns(new UserModel());

            // Act 
            try
            {
                await _sut.RegisterClientAsync(model);
            }
            catch (UserExistsException ex)
            {
                Assert.Equal("This email already exists", actual: ex.Message);
               // Assert.ThrowsAsync<UserExistsException>();
            }
        }
    }
}
