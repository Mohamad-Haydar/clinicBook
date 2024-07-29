using api.BusinessLogic.DataAccess;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Data;
using api.Exceptions;
using api.Models;
using api.Models.Request;
using api.Models.Responce;
using API.Tests.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Tests.BusinessLogic
{
    public class DoctorAvailabilityTests
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly DbFactory _contextFactory;
        private readonly IDoctorAvailabilityData _sut;
        private readonly ApplicationDbContext _appContext;

        public DoctorAvailabilityTests()
        {
            _contextFactory = new DbFactory();
            _userManager = _contextFactory.CreateUserManager();
            _appContext = _contextFactory.CreateAppContext();
            _sut = new DoctorAvailabilityData(_userManager, _appContext);
        }

        [Fact]
        public async Task GetAvailableDatesAsync_DoctorFound_GetTheDoctorAvailability()
        {
            //Arrange
            string doctorId = "id1";
            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities =
           [
               new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id2",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
           ];
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var result = await _sut.GetAvailableDatesAsync(doctorId);

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

        [Fact]
        public async Task GetAvailableDatesAsync_DoctorNotFound_GetEmptyResult()
        {
            // Arrange
            string doctorId = "doctorid";
            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities =
           [
               new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id2",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                 new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
           ];
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var result = await _sut.GetAvailableDatesAsync(doctorId);

            // Assert
            var availabilityList = Assert.IsAssignableFrom<List<DoctorAvailabilityResponse>>(result).ToList();
            Assert.Empty(availabilityList);
        }

        [Fact]
        public async Task OpenAvailableDateAsync_DoctorNotFound_ThrowNotFoundException()
        {
            // Arrange
            OpenAvailableDateRequest model = new()
            {
                AvailableDate = new DateOnly(2024,12,7),
                StartHour = new TimeSpan(8),
                EndHour = new TimeSpan(8),
                MaxClient = 20,
                DoctorId = "doctorId",
            };
            _userManager.FindByIdAsync(model.DoctorId).ReturnsNull();

            // Act
            NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(() => _sut.OpenAvailableDateAsync(model));

            // Assert
            Assert.Equal("Doctor Not Found please enter a valid doctor", exception.Message);

        }

        [Fact]
        public async Task OpenAvailableDateAsync_AddedSuccessfully()
        {
            // Arrange
            OpenAvailableDateRequest model = new()
            {
                AvailableDate = new DateOnly(2024, 12, 7),
                StartHour = new TimeSpan(12),
                EndHour = new TimeSpan(8),
                MaxClient = 20,
                DoctorId = "doctorId",
            };
            _userManager.FindByIdAsync(model.DoctorId).Returns(new UserModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.OpenAvailableDateAsync(model));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task UpdateAvailableDateAsync_AvailabilityNotFound_ThrowNotFoundExceptoin()
        {
            // Arrange
            UpdateAvailableDateRequest model = new()
            {
                Id = 1,
                AvailableDate = new DateOnly(2024, 7, 12),
                StartHour = new TimeSpan(9),
                EndHour = new TimeSpan(12),
                MaxClient = 20,
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.UpdateAvailableDateAsync(model));

            // Assert
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal("their is no available date to update",exception.Message);
        }

        [Fact]
        public async Task UpdateAvailableDateAsync_InvalidStartEndHour_ThrowNotFoundExceptoin()
        {
            // Arrange
            UpdateAvailableDateRequest model = new()
            {
                Id = 1,
                AvailableDate = new DateOnly(2024, 7, 12),
                StartHour = new TimeSpan(12),
                EndHour = new TimeSpan(8),
                MaxClient = 20,
            };
            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities =
           [
               new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id2",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
           ];
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.UpdateAvailableDateAsync(model));

            // Assert
            Assert.IsType<InvalidDataException>(exception);
            Assert.Equal("check your dates start date should be less that end date", exception.Message);
        }

        [Fact]
        public async Task UpdateAvailableDateAsync_InvalidAvailableDate_ThrowNotFoundExceptoin()
        {
            // Arrange
            UpdateAvailableDateRequest model = new()
            {
                Id = 1,
                AvailableDate = new DateOnly(2024, 1, 12),
                StartHour = new TimeSpan(8),
                EndHour = new TimeSpan(9),
                MaxClient = 20,
            };
            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities =
           [
               new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id2",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
           ];
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.UpdateAvailableDateAsync(model));

            // Assert
            Assert.IsType<InvalidDataException>(exception);
            Assert.Equal("check availability date should not be previouse today", exception.Message);
        }

        [Fact]
        public async Task UpdateAvailableDateAsync_UpdatedSuccessfully()
        {
            // Arrange
            UpdateAvailableDateRequest model = new()
            {
                Id = 1,
                AvailableDate = new DateOnly(2024, 12, 12),
                StartHour = new TimeSpan(8),
                EndHour = new TimeSpan(9),
                MaxClient = 20,
            };
            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities =
           [
               new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id2",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
           ];
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.UpdateAvailableDateAsync(model));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task DeleteAvailableDateAsync_AvailabilityNotFound_ThrowNotFoundExceptoin()
        {
            // Arrange
            int availabilityId = 1;

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.DeleteAvailableDateAsync(availabilityId));

            // Assert
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal("Not Found, Enter a valid input", exception.Message);
        }

        [Fact]
        public async Task DeleteAvailableDateAsync_DeletedSuccessfully()
        {
            // Arrange
            int availabilityId = 1;
            IEnumerable<DoctorAvailabilityModel> doctorAvailabilities =
           [
               new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 12),
                    DayName = "Thursday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id1",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                },
                new()
                {
                    DoctorId = "id2",
                    AvailableDate = new DateOnly(2024, 7, 13),
                    DayName = "Friday",
                    StartHour = new TimeSpan(8, 0, 0),
                    EndHour = new TimeSpan(12, 0, 0),
                    MaxClient = 20
                }
           ];
            _appContext.DoctorAvailabilities.AddRange(doctorAvailabilities);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.DeleteAvailableDateAsync(availabilityId));
            var res = await _sut.GetAvailableDatesAsync("id1");

            // Assert
            Assert.Null(exception);
            Assert.Single(res);
            Assert.Equal("Friday", res.First().dayName);
        }

    }
}
