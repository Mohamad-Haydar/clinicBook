using api.BusinessLogic.DataAccess;
using api.Controllers;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using API.Tests.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
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
        private readonly DbFactory _contextFactory;


        public AuthenticationDataTests()
        {
            _contextFactory = new DbFactory();
            _userManager = _contextFactory.CreateUserManager();
            _appContext = _contextFactory.CreateAppContext();
            _identityContext = _contextFactory.CreateIdentityContext();
            _roleManager = _contextFactory.CreateRoleManager();
            _tokenService = Substitute.For<ITokenService>();
            
            _sut = new AuthenticationData(_identityContext, _userManager, _roleManager, _appContext, _tokenService);
        }

        [Fact]
        public async Task RegisterClientAsync_UserAlreadyExists_ThrowUserExistsException()
        {
            // Arrange
            CreateUserRequest model = new() { Email = "someemail" };
            _userManager.FindByEmailAsync("someemail").Returns(new UserModel());

            // Act 
            UserAlreadyExistsException exception = await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _sut.RegisterClientAsync(model));

            // Assert
            Assert.Equal("This email already exists", exception.Message);
        }

       
    }
}
