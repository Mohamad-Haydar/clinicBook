using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Tests.Controllers
{
    public class DoctorManagementControllerTests
    {
        private readonly IDoctorManagementData _doctorAvailabilityData = Substitute.For<IDoctorManagementData>();
        private readonly DoctorManagementController _sut;

        public DoctorManagementControllerTests()
        {
            _sut = new DoctorManagementController(_doctorAvailabilityData);
        }

        [Fact]
        public void DoctorServiceRequest_InvalidModel_EmptyModel()
        {
            // Arrange
            DoctorServiceRequest model = new();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotNull(results);

            // check spesific validation errors
            var errorLists = results.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("Duration is required and should be greater than 0", errorLists);
            Assert.Contains("Doctor Id is required", errorLists);
            Assert.Contains("service id is required and should be greater than 0", errorLists);

        }

        [Fact]
        public async Task AddDoctorService_FailedLogic()
        {
            // Arrange 
            DoctorServiceRequest model = new();
            _doctorAvailabilityData.AddDoctorServiceAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.AddDoctorService(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("check your dates start date should be less that end date, and check availability date should not be previouse today", badResponse.Message);
        }

        [Fact]
        public async Task AddDoctorService_AccessLogic()
        {
            // Arrange
            DoctorServiceRequest model = new();
            _doctorAvailabilityData.AddDoctorServiceAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.AddDoctorService(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okResult.Value as Response;
            Assert.NotNull(okResponse);
            Assert.Equal("Service Added Successfully to the doctor", okResponse.Message);
        }
    
        [Fact]
        public async Task AddMultipleService_FaildLogic()
        {
            // Arrange 
            List<DoctorServiceRequest> models = new();
            _doctorAvailabilityData.AddMultipleServiceAsync(models).Throws(new Exception());

            // Act
            var result = await _sut.AddMultipleService(models);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("check your dates start date should be less that end date, and check availability date should not be previouse today", badResponse.Message);
        }

        [Fact]
        public async Task AddMultipleService_AccessLogic()
        {
            // Arrange 
            List<DoctorServiceRequest> models = new();
            _doctorAvailabilityData.AddMultipleServiceAsync(models).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.AddMultipleService(models);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okObjectResult.Value as Response;
            Assert.NotNull(okResponse);
            Assert.Equal("All Services added successfully", okResponse.Message);
        
        }

        [Fact]
        public async Task UpdateDoctorServiceDuration_FailedLogic()
        {
            // Given
            int id = 1, duration = 15;
            _doctorAvailabilityData.UpdateDoctorServiceDurationAsync(id, duration).Throws(new Exception());

            // When
            var result = await _sut.UpdateDoctorServiceDuration(id, duration);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again.", badResponse.Message);
        }

        [Fact]
        public async Task UpdateDoctorServiceDuration_AccessdLogic()
        {
            // Given
            int id = 1, duration = 15;
            _doctorAvailabilityData.UpdateDoctorServiceDurationAsync(id, duration).Returns(Task.CompletedTask);

            // When
            var result = await _sut.UpdateDoctorServiceDuration(id, duration);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as Response;
            Assert.NotNull(okResponse);
            Assert.Equal("Service Duration updated successfully", okResponse.Message);
        }

        [Fact]
        public async Task DeleteDoctorService_FailedLogic()
        {
            // Given
            int id = 1;
            _doctorAvailabilityData.DeleteDoctorServiceAsync(id).Throws(new Exception());

            // When
            var result = await _sut.DeleteDoctorService(id);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again.", badResponse.Message);
        }

        [Fact]
        public async Task DeleteDoctorService_AccessdLogic()
        {
            // Given
            int id = 1;
            _doctorAvailabilityData.DeleteDoctorServiceAsync(id).Returns(Task.CompletedTask);

            // When
            var result = await _sut.DeleteDoctorService(id);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as Response;
            Assert.NotNull(okResponse);
            Assert.Equal("Service Deleted successfully", okResponse.Message);
        }

        [Fact]
        public async Task RemoveDoctor_FailedLogic()
        {
            // Given
            string id = "some id";
            _doctorAvailabilityData.RemoveDoctorAsync(id).Throws(new Exception());

            // When
            var result = await _sut.RemoveDoctor(id);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again.", badResponse.Message);
        }

        [Fact]
        public async Task RemoveDoctor_AccessdLogic()
        {
            // Given
            string id = "some id";
            _doctorAvailabilityData.RemoveDoctorAsync(id).Returns(Task.CompletedTask);

            // When
            var result = await _sut.RemoveDoctor(id);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as Response;
            Assert.NotNull(okResponse);
            Assert.Equal("doctor removed successfully", okResponse.Message);
        }

        [Fact]
        public async Task UpdateDoctorInfo_FailedLogic()
        {
            // Given
            CreateDoctorRequest model = new();
            _doctorAvailabilityData.UpdateDoctorInfoAsync(model).Throws(new Exception());

            // When
            var result = await _sut.UpdateDoctorInfo(model);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again.", badResponse.Message);
        }

        [Fact]
        public async Task UpdateDoctorInfo_AccessdLogic()
        {
            // Given
            CreateDoctorRequest model = new();
            _doctorAvailabilityData.UpdateDoctorInfoAsync(model).Returns(Task.CompletedTask);

            // When
            var result = await _sut.UpdateDoctorInfo(model);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as Response;
            Assert.NotNull(okResponse);
            Assert.Equal("Doctor Data updated successfully.", okResponse.Message);
        }

        [Fact]
        public async Task GetDoctorByEmail_FailedLogic()
        {
            // Given
            string email = "email@gmail.com";
            _doctorAvailabilityData.GetDoctorByEmailAsync(email).Throws(new Exception());

            // When
            var result = await _sut.GetDoctorByEmail(email);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again.", badResponse.Message);
        }

        [Fact]
        public async Task GetDoctorByEmail_AccessdLogic()
        {
            // Given
            string email = "email@gmail.com";
            var reservations = new DoctorInfoResponce(){Email = "email@gmail.com" };
            _doctorAvailabilityData.GetDoctorByEmailAsync(email).Returns(Task.FromResult(reservations));

            // When
            var result = await _sut.GetDoctorByEmail(email);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as DoctorInfoResponce;
            Assert.NotNull(okResponse);
        }

        [Fact]
        public async Task GetDoctorById_FailedLogic()
        {
            // Given
            string id = "someId";
            _doctorAvailabilityData.GetDoctorByIdAsync(id).Throws(new Exception());

            // When
            var result = await _sut.GetDoctorById(id);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again", badResponse.Message);
        }

        [Fact]
        public async Task GetDoctorById_AccessdLogic()
        {
            // Given
            string id = "someId";
            var reservations = new DoctorInfoResponce() { Id = "someId" };
            _doctorAvailabilityData.GetDoctorByIdAsync(id).Returns(Task.FromResult(reservations));

            // When
            var result = await _sut.GetDoctorById(id);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as DoctorInfoResponce;
            Assert.NotNull(okResponse);
        }

        [Fact]
        public async Task GetAllDoctorsNameAndId_FailedLogic()
        {
            // Given
            _doctorAvailabilityData.GetAllDoctorsNameAndIdAsync().Throws(new Exception());

            // When
            var result = await _sut.GetAllDoctorsNameAndId();

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again", badResponse.Message);
        }

        [Fact]
        public async Task GetAllDoctorsNameAndId_AccessdLogic_NoContent()
        {
            // Given
            IEnumerable<DoctorNameResponse> reservations = new List<DoctorNameResponse>();
            _doctorAvailabilityData.GetAllDoctorsNameAndIdAsync().Returns(Task.FromResult(reservations));

            // When
            var result = await _sut.GetAllDoctorsNameAndId();

            // Then
            var noContent = Assert.IsType<NoContentResult>(result);
            Assert.NotNull(noContent);
        }

        [Fact]
        public async Task GetAllDoctorsNameAndId_AccessdLogic()
        {
            // Given
            IEnumerable<DoctorNameResponse> reservations = new List<DoctorNameResponse>() { new(), new() };
            _doctorAvailabilityData.GetAllDoctorsNameAndIdAsync().Returns(Task.FromResult(reservations));

            // When
            var result = await _sut.GetAllDoctorsNameAndId();

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as IEnumerable<DoctorNameResponse>;
            Assert.NotNull(okResponse);
            Assert.Equal(2, okResponse.Count());
        }

        [Fact]
        public async Task GetDoctorsByCategory_FailedLogic()
        {
            // Given
            int categoryId = 1;
            _doctorAvailabilityData.GetDoctorsByCategoryAsync(categoryId).Throws(new Exception());

            // When
            var result = await _sut.GetDoctorsByCategory(categoryId);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badResponse = badRequestResult.Value as Response;
            Assert.NotNull(badResponse);
            Assert.Equal("Something whent wrong, please try again", badResponse.Message);
        }

        [Fact]
        public async Task GetDoctorsByCategory_AccessdLogic_NoContent()
        {
            // Given
            int categoryId = 1;
            IEnumerable<DoctorInfoResponce> reservations = [];
            _doctorAvailabilityData.GetDoctorsByCategoryAsync(categoryId).Returns(Task.FromResult(reservations));

            // When
            var result = await _sut.GetDoctorsByCategory(categoryId);

            // Then
            var noContent = Assert.IsType<NoContentResult>(result);
            Assert.NotNull(noContent);
        }

        [Fact]
        public async Task GetDoctorsByCategory_AccessdLogic()
        {
            // Given
            int categoryId = 1;
            IEnumerable<DoctorInfoResponce> reservations = [new(), new()];
            _doctorAvailabilityData.GetDoctorsByCategoryAsync(categoryId).Returns(Task.FromResult(reservations));

            // When
            var result = await _sut.GetDoctorsByCategory(categoryId);

            // Then
            var okRequestResult = Assert.IsType<OkObjectResult>(result);
            var okResponse = okRequestResult.Value as IEnumerable<DoctorInfoResponce>;
            Assert.NotNull(okResponse);
            Assert.Equal(2, okResponse.Count());
        }

    }
}
