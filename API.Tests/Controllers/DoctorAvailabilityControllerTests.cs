using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Exceptions;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.ComponentModel.DataAnnotations;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Tests.Controllers
{
    public class DoctorAvailabilityControllerTests
    {
        private readonly DoctorAvailabilityController _sut;
        private readonly IDoctorAvailabilityData _doctorAvailabilityData = Substitute.For<IDoctorAvailabilityData>();
        public DoctorAvailabilityControllerTests()
        {
            _sut = new DoctorAvailabilityController(_doctorAvailabilityData);
        }

        [Fact]
        public async Task GetAvailableDates_ValidModel_FailedLogic()
        {
            // Arrange
            string id = "some random id";
            _doctorAvailabilityData.GetAvailableDatesAsync(id).Throws(new Exception());

            // Act
            var result = await _sut.GetAvailableDates(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var BadResponse = badRequestResult.Value as Response;
            Assert.NotNull(BadResponse);
            Assert.Equal("Something went wrong. Please try again.", BadResponse.Message);
        }

        [Fact]
        public async Task GetAvailableDates_ValidModel_FailedInternalLogic()
        {
            // Arrange
            string id = "some random id";
            _doctorAvailabilityData.GetAvailableDatesAsync(id).Throws(new BusinessException("An error occurred while registering the client"));

            // Act
            var result = await _sut.GetAvailableDates(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var BadResponse = badRequestResult.Value as Response;
            Assert.NotNull(BadResponse);
            Assert.Equal("An error occurred while registering the client", BadResponse.Message);
        }

        [Fact]
        public async Task GetAvailableDates_ValidModel_AccessLogic()
        {
            // Arrange
            string id = "some random id";
            _doctorAvailabilityData.GetAvailableDatesAsync(id).Returns(x => []);

            // Act
            var result = await _sut.GetAvailableDates(id);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var OkResponse = badRequestResult.Value as object;
            Assert.NotNull(OkResponse);
        }

        [Fact]
        public void OpenAvailableDateRequest_Invalid_EmptyModel()
        {
            // Arrange
            OpenAvailableDateRequest model = new();
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x =>  x.ErrorMessage).ToList();
            Assert.Contains("Date must be in the future", errorLists);
            Assert.Contains("Start hour must be between 7 AM and 8 PM", errorLists);
            Assert.Contains("End hour must be between 7 AM and 8 PM", errorLists);
            Assert.Contains("Number of people must be greater than 0", errorLists);
            Assert.Contains("DoctorId is required", errorLists);
        }

        [Fact]
        public void OpenAvailableDateRequest_Invalid_FalseData()
        {
            // Arrange
            OpenAvailableDateRequest model = new()
            {
                AvailableDate = new DateOnly(2022,12,12),
                StartHour = new TimeSpan(9,0,0),
                EndHour = new TimeSpan(7,0,0),
                MaxClient = 12,
                DoctorId = "some random id"

            };
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("Date must be in the future", errorLists);
            Assert.Contains("Start hour must be less than end hour", errorLists);
            Assert.Equal(2, errorLists.Count());
        }

        [Fact]
        public async Task OpenAvailableDate_FailedLogic()
        {
            // Arrange
            OpenAvailableDateRequest model = new();
            _doctorAvailabilityData.OpenAvailableDateAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.OpenAvailableDate(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = badRequestResult.Value as Response;
            Assert.NotNull(errorResponse);
            Assert.Equal("something when wrong please try again", errorResponse.Message);
        }

        [Fact]
        public async Task OpenAvailableDate_AccessLogic()
        {
            // Arrange
            OpenAvailableDateRequest model = new();
            _doctorAvailabilityData.OpenAvailableDateAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.OpenAvailableDate(model);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var errorResponse = badRequestResult.Value as Response;
            Assert.NotNull(errorResponse);
            Assert.Equal("Available date added successfully", errorResponse.Message);
        }

        [Fact]
        public void UpdateAvailableDateRequest_Invalid_EmptyModel()
        {
            // Arrange
            UpdateAvailableDateRequest model = new();
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("Id is required and it should be greater than 0", errorLists);
            Assert.Contains("Date must be in the future", errorLists);
            Assert.Contains("Start hour must be between 7 AM and 8 PM", errorLists);
            Assert.Contains("End hour must be between 7 AM and 8 PM", errorLists);
            Assert.Contains("Number of people must be greater than 0", errorLists);
        }

        [Fact]
        public void UpdateAvailableDateRequest_Invalid_FalseData()
        {
            // Arrange
            UpdateAvailableDateRequest model = new()
            {
                AvailableDate = new DateOnly(2022, 12, 12),
                StartHour = new TimeSpan(9, 0, 0),
                EndHour = new TimeSpan(7, 0, 0),
                MaxClient = 12,
            };
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("Date must be in the future", errorLists);
            Assert.Contains("Start hour must be less than end hour", errorLists);
            Assert.Contains("Id is required and it should be greater than 0", errorLists);
            Assert.Equal(3, errorLists.Count());
        }

        [Fact]
        public async Task UpdateAvailableDate_FailedLogic()
        {
            // Arrange
            UpdateAvailableDateRequest model = new();
            _doctorAvailabilityData.UpdateAvailableDateAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.UpdateAvailableDate(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = badRequestResult.Value as Response;
            Assert.NotNull(errorResponse);
            Assert.Equal("Something when wrong, please try again", errorResponse.Message);
        }

        [Fact]
        public async Task UpdateAvailableDate_AccessLogic()
        {
            // Arrange
            UpdateAvailableDateRequest model = new();
            _doctorAvailabilityData.UpdateAvailableDateAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateAvailableDate(model);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var errorResponse = badRequestResult.Value as Response;
            Assert.NotNull(errorResponse);
            Assert.Equal("Available date added successfully", errorResponse.Message);
        }

        [Fact]
        public async Task DeleteAvailableDate_FailedLogic()
        {
            // Arrange
            int id = 1;
            _doctorAvailabilityData.DeleteAvailableDateAsync(id).Throws(new Exception());

            // Act
            var result = await _sut.DeleteAvailableDate(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = badRequestResult.Value as Response;
            Assert.NotNull(errorResponse);
            Assert.Equal("something when wrong please try again", errorResponse.Message);
        }

        [Fact]
        public async Task DeleteAvailableDate_AccessLogic()
        {
            // Arrange
           int id = 1;
            _doctorAvailabilityData.DeleteAvailableDateAsync(id).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteAvailableDate(id);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var errorResponse = badRequestResult.Value as Response;
            Assert.NotNull(errorResponse);
            Assert.Equal("availabel date removed successfully", errorResponse.Message);
        }



    }
}
