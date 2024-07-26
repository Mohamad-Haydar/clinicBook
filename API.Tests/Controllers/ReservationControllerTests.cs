using Xunit;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using api.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;
using api.Models.Responce;
using NSubstitute.ReturnsExtensions;

namespace API.Tests.Controllers.Tests;

public class ReservationControllerTests
{
        private readonly ReservationController _sut;
        private readonly IReservationData _reservationData = Substitute.For<IReservationData>();

        public ReservationControllerTests()
        {
                _sut = new ReservationController(_reservationData);
        }

        [Fact]
        public void CreateQueueReservation_InvalidModel_ReturnsBadRequest()
        {
                // Arrange
                var model = new CreateQueueReservationRequest(); // All properties are null or default
                var context = new ValidationContext(model);
                var results = new List<ValidationResult>();

                // Act
                var isModelStateValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

                // Assert
                Assert.False(isModelStateValid); // Model state should be invalid
                Assert.NotEmpty(results); // There should be validation errors

                // Check specific validation errors
                var errorMessages = results.Select(r => r.ErrorMessage).ToList();
                Assert.Contains("client_id is required", errorMessages);
                Assert.Contains("Enter a valid availability", errorMessages);
                Assert.Contains("doctor_service_ids is required", errorMessages);
        }

        [Fact]
        public void CreateQueueReservation_InvalidModel_ReturnsOK()
        {
                // Arrange
                var model = new CreateQueueReservationRequest()
                {
                        client_id = "saddsa",
                        doctor_availability_id = 1,
                        doctor_service_ids = [1, 2]
                };
                var context = new ValidationContext(model, serviceProvider: null, items: null);
                var results = new List<ValidationResult>();

                // Act
                var isModelStateValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

                // Assert
                Assert.True(isModelStateValid); // Model state should be invalid
                Assert.Empty(results); // There should be validation errors
        }

        [Fact]
        public async Task CreateQueueReservation_ValidModel_RunsBusinessLogic()
        {
                // Arrange
                var model = new CreateQueueReservationRequest() { };
                _reservationData.CreateQueueReservationAsync(model).Returns(Task.CompletedTask);

                // Act
                var result = await _sut.CreateQueueReservation(model);

                // Assert
                var badRequestResult = Assert.IsType<OkObjectResult>(result);
                var errorResponse = badRequestResult.Value as Reseponce;
                Assert.NotNull(errorResponse);
                Assert.Equal("Reservation added successfully", errorResponse.Message);
        }

        [Fact]
        public async Task CreateQueueReservation_ValidModel_FailedLogic()
        {
                // Arrange
                var model = new CreateQueueReservationRequest() { };
                _reservationData.CreateQueueReservationAsync(model).Returns(x => { throw new Exception(); });

                // Act
                var result = await _sut.CreateQueueReservation(model);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var errorResponse = badRequestResult.Value as Reseponce;
                Assert.NotNull(errorResponse);
                Assert.Equal("An error occurred while processing your request.", errorResponse.Message);
        }


}