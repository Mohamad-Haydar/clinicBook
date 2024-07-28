using api.BusinessLogic.DataAccess;
using api.Data;
using api.Models;
using api.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using NSubstitute;
using NSubstitute.Core.Arguments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests.BusinessLogic
{
    public class DoctorAvailabilityTests
    {
        private readonly IdentityAppDbContext _identityContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _appContext;
        private readonly ITokenService _tokenService;
        private readonly DoctorAvailabilityData _sut;
        private readonly DbContextOptions<ApplicationDbContext> _appOptions;
        private readonly DbContextOptions<IdentityAppDbContext> _identityOptions;

        public DoctorAvailabilityTests()
        {
            // Create context options for both database
            _appOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "clinicbook")
            .Options;

            _identityOptions = new DbContextOptionsBuilder<IdentityAppDbContext>()
            .UseInMemoryDatabase(databaseName: "clinicusers")
            .Options;
            // create the context for both database
            _appContext = new ApplicationDbContext(_appOptions);
            _identityContext = new IdentityAppDbContext(_identityOptions);

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

            _sut = new DoctorAvailabilityData(_identityContext, _userManager, _roleManager, _appContext, _tokenService);
        }

        [Fact]
        public async Task GetAvailableDates_DoctorNotFound_ThroeException2()
        {
            string doctorId = "someid";

            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities = new List<DoctorAvailabilityModel>
            {
                new DoctorAvailabilityModel
                {
                    DoctorId = doctorId,
                    Id = 1,
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new DoctorAvailabilityModel
                {
                    DoctorId = doctorId,
                    Id = 2,
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
            };
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var result = await _sut.GetAvailableDates(doctorId);

            // Assert
            var availabilityList = Assert.IsAssignableFrom<List<DoctorAvailabilityResponse>>(result).ToList();
            Assert.Equal(2, availabilityList.Count);
            Assert.Equal(1, availabilityList[0].id);
            Assert.Equal(new DateOnly(2024, 7, 12), availabilityList[0].day);
            Assert.Equal("Thursday", availabilityList[0].dayName);
            Assert.Equal(new TimeSpan(8, 0, 0), availabilityList[0].startHour);
            Assert.Equal(new TimeSpan(12, 0, 0), availabilityList[0].endHour);
            Assert.Equal(20, availabilityList[0].maxClient);
        }

    }
}
