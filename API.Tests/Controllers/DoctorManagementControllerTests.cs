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
    }
}
