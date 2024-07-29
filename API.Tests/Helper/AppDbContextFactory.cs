using api.Data;
using api.Models;
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

namespace API.Tests.Helper
{
    public class AppDbContextFactory
    {
        public ApplicationDbContext CreateAppContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        public IdentityAppDbContext CreateIdentityContext()
        {
            var options = new DbContextOptionsBuilder<IdentityAppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new IdentityAppDbContext(options);
        }

        public UserManager<UserModel> CreateUserManager()
        {
            var userStore = Substitute.For<IUserStore<UserModel>>();
            var options = Substitute.For<IOptions<IdentityOptions>>();
            var passwordHasher = Substitute.For<IPasswordHasher<UserModel>>();
            var userValidators = new List<IUserValidator<UserModel>> { Substitute.For<IUserValidator<UserModel>>() };
            var passwordValidators = new List<IPasswordValidator<UserModel>> { Substitute.For<IPasswordValidator<UserModel>>() };
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var services = Substitute.For<IServiceProvider>();
            var logger = Substitute.For<ILogger<UserManager<UserModel>>>();

            return Substitute.For<UserManager<UserModel>>(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger);
        }

        public RoleManager<IdentityRole> CreateRoleManager()
        {
            var roleStore = Substitute.For<IRoleStore<IdentityRole>>();
            var keyNormalizer = Substitute.For<ILookupNormalizer>();
            var errors = Substitute.For<IdentityErrorDescriber>();
            var loggerRole = Substitute.For<ILogger<RoleManager<IdentityRole>>>();
            var roleValidators = new List<IRoleValidator<IdentityRole>>();

            return Substitute.For<RoleManager<IdentityRole>>(roleStore, roleValidators, keyNormalizer, errors, loggerRole);
        }
    }
}
