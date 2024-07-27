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
using NSubstitute.ExceptionExtensions;
using api.Models;

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
                var model = new CreateQueueReservationRequest()
                {
                    client_id = null,
                    doctor_availability_id = null,
                    doctor_service_ids = null,
                }; // All properties are null or default
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
                Assert.Contains("doctor_availability_id is required", errorMessages);
                Assert.Contains("doctor_service_ids is required", errorMessages);
        }

        [Fact]
        public void CreateQueueReservation_InvalidModel_invalidId_ReturnsBadRequest()
        {
            // Arrange
            var model = new CreateQueueReservationRequest()
            {
                client_id = "fdsaf",
                doctor_availability_id = 0,
                doctor_service_ids = [1],
            }; // All properties are null or default
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isModelStateValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            // Assert
            Assert.False(isModelStateValid); // Model state should be invalid
            Assert.NotEmpty(results); // There should be validation errors

            // Check specific validation errors
            var errorMessages = results.Select(r => r.ErrorMessage).ToList();
            Assert.Contains("Enter a valid availability", errorMessages);
        }

        [Fact]
        public void CreateQueueReservation_ValidModel_ReturnsOK()
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
                var Responce = badRequestResult.Value as Response;
                Assert.NotNull(Responce);
                Assert.Equal("Reservation added successfully", Responce.Message);
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
                var errorResponse = badRequestResult.Value as Response;
                Assert.NotNull(errorResponse);
                Assert.Equal("An error occurred while processing your request.", errorResponse.Message);
        }

        [Fact]
        public async Task GetReservationDetails_ValidModel_FailedLogic()
        {
            // Arrange
            int id = 1;
            _reservationData.GetReservationDetailsAsync(id).ThrowsAsync(x => new Exception());
            
            // Act
            var result = await _sut.GetReservationDetails(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("Something whent wrong please try again.", Responce.Message);
        }

        [Fact]
        public async Task GetReservationDetails_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            int id =1;
            _reservationData.GetReservationDetailsAsync(id).Returns(x => []);

            // Act
            var result = await _sut.GetReservationDetails(id);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            var Responce = badRequestResult.Value as Dictionary<string, object>;
            Assert.NotNull(Responce);
        }

        [Fact]
        public async Task GetAllPersonalReservations_ValidModel_FailedLogic()
        {
            // Arrange
            string clientId = "randomid";
            _reservationData.GetAllPersonalReservationsAsync(clientId).Throws(x => new Exception());

            // Act
            var result = await _sut.GetAllPersonalReservations(clientId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("something when wrong please check you input and try again", Responce.Message);
        }

        [Fact]
        public async Task GetAllPersonalReservations_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            string clientId = "randomid";
            var reservations = new List<Dictionary<string, object>>
            {
                new() { { "reservationId", 1 }, { "clientId", clientId } }
            }.AsQueryable();
            _reservationData.GetAllPersonalReservationsAsync(clientId).Returns(Task.FromResult(reservations));

            // Act
            var result = await _sut.GetAllPersonalReservations(clientId);

            // Assert
            var RequestResult = Assert.IsType<OkObjectResult>(result);
            var Responce = RequestResult.Value;
            Assert.NotNull(Responce);
        Assert.Equal(Responce, reservations);
        }

        [Fact]
        public async Task GetConcurrentBookings_ValidModel_FailedLogic()
        {
            // Arrange
            int clientReservationId = 1;
            _reservationData.GetConcurrentBookingsAsync(clientReservationId).Throws(new Exception());

            // Act
            var result = await _sut.GetConcurrentBookings(clientReservationId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("something when wrong please check you input and try again", Responce.Message);

        }

        [Fact]
        public async Task GetConcurrentBookings_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            int clientReservationId = 1;
            var reservations = new List<Dictionary<string, object>>
                {
                    new() { { "reservationId", 1 }, { "clientId", clientReservationId } }
                }.AsQueryable();
            _reservationData.GetConcurrentBookingsAsync(clientReservationId).Returns(Task.FromResult(reservations));

            // Act
            var result = await _sut.GetConcurrentBookings(clientReservationId);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var Responce = OkResult.Value;
            Assert.NotNull(Responce);
            Assert.Equal(reservations, Responce);
        }

        [Fact]
        public async Task GetPreviousBookings_ValidModel_FailedLogic()
        {
            // Arrange
            int clientReservationId = 1;
            _reservationData.GetPreviousBookingsAsync(clientReservationId).Throws(new Exception());

            // Act
            var result = await _sut.GetPreviousBookings(clientReservationId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("something when wrong please check you input and try again", Responce.Message);
        }

        [Fact]
        public async Task GetPreviousBookings_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            int clientReservationId = 1;
            var reservations = new List<Dictionary<string, object>>
                    {
                        new() { { "reservationId", 1 }, { "clientId", clientReservationId } }
                    }.AsQueryable();
            _reservationData.GetPreviousBookingsAsync(clientReservationId).Returns(Task.FromResult(reservations));

            // Act
            var result = await _sut.GetPreviousBookings(clientReservationId);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var Responce = OkResult.Value as IQueryable<Dictionary<string, object>>;
            Assert.NotNull(Responce);
            Assert.Equal(2, Responce.First().Count());
            Assert.Equal(reservations, Responce);
    }

        [Fact]
        public async Task DeleteSpecificReservation_ValidModel_FailedLogic()
        {
            // Arrange
            int clientReservationId = 1;
            _reservationData.DeleteSpecificReservationAsync(clientReservationId).Throws(new Exception());

            // Act
            var result = await _sut.DeleteSpecificReservation(clientReservationId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("something when wrong please check you input and try again", Responce.Message);
        }

        [Fact]
        public async Task DeleteSpecificReservation_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            int clientReservationId = 1;
            _reservationData.DeleteSpecificReservationAsync(clientReservationId).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteSpecificReservation(clientReservationId);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var Responce = OkResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("your reservation is removed successfully", Responce.Message);
        }

        [Fact] public void DeleteSpecificReservation_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            UpdateReservationRequest model = new();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isModelStateValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            // Assert
            Assert.False(isModelStateValid);
            Assert.NotEmpty(results);

            // Check the results content
            var errorMessages = results.Select(r => r.ErrorMessage).ToList();
            Assert.Contains("client reservation id is required", errorMessages);
            Assert.Contains("doctor service ids are required", errorMessages);
        }

        [Fact]
        public void DeleteSpecificReservation_InvalidModel_FalseId_ReturnsBadRequest()
        {
            // Arrange
            UpdateReservationRequest model = new() { doctor_service_ids = [], client_reservation_id = 0 };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isModelStateValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            // Assert
            Assert.False(isModelStateValid);
            Assert.NotEmpty(results);

            // Check the results content
            var errorMessages = results.Select(r => r.ErrorMessage).ToList();
            Assert.Contains("client reservation id should be greater than 0", errorMessages);
            Assert.Single(errorMessages);
        }

        [Fact]
        public void DeleteSpecificReservation_ValidModel_ContinueToBusinessLogic()
        {
            // Arrange
            UpdateReservationRequest model = new() { client_reservation_id = 1, doctor_service_ids = [1,2] };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isModelStateValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            // Assert
            Assert.True(isModelStateValid);
            Assert.Empty(results);
        }

        [Fact]
        public async Task UpdateSpecificReservation_ValidModel_FailedLogic()
        {
            // Arrange
            UpdateReservationRequest model = new();
            _reservationData.UpdateSpecificReservationAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.UpdateSpecificReservation(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("Please check your input and try again", Responce.Message);
        }

        [Fact]
        public async Task UpdateSpecificReservation_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            UpdateReservationRequest model = new();
            _reservationData.UpdateSpecificReservationAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateSpecificReservation(model);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var Responce = OkResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("your reservation is Updated successfully", Responce.Message);
        }

        [Fact]
        public async Task GetAllReservationForTheDay_ValidModel_FailedLogic()
        {
            // Arrange
            int DoctorAvailabilityId = 1;
            _reservationData.GetAllReservationForTheDayAsync(DoctorAvailabilityId).Throws(new Exception());

            // Act
            var result = await _sut.GetAllReservationForTheDay(DoctorAvailabilityId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("something when wrong please check you input and try again", Responce.Message);
        }

        [Fact]
        public async Task GetAllReservationForTheDay_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            int DoctorAvailabilityId = 1;
            var reservations = new List<Dictionary<string, object>>
            {
                new() { { "reservationId", 1 }, { "clientId", 2 } }
            }.AsQueryable();
            _reservationData.GetAllReservationForTheDayAsync(DoctorAvailabilityId).Returns(Task.FromResult(reservations));

            // Act
            var result = await _sut.GetAllReservationForTheDay(DoctorAvailabilityId);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var Responce = OkResult.Value as IQueryable<Dictionary<string, object>>;
            Assert.NotNull(Responce);
            Assert.Equal(2, Responce.First().Count());
            Assert.Equal(reservations, Responce);
        }

        [Fact]
        public async Task MarkCompleteReservation_ValidModel_FailedLogic()
        {
            // Arrange
            int ClientReservationId = 1;
            _reservationData.MarkCompleteReservationAsync(ClientReservationId).Throws(new Exception());

            // Act
            var result = await _sut.MarkCompleteReservation(ClientReservationId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var Responce = badRequestResult.Value as Response;
            Assert.NotNull(Responce);
            Assert.Equal("something when wrong please check you input and try again", Responce.Message);
        }

        [Fact]
        public async Task MarkCompleteReservation_ValidModel_RunsBusinessLogic()
        {
            // Arrange
            int ClientReservationId = 1;
            _reservationData.MarkCompleteReservationAsync(ClientReservationId).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.MarkCompleteReservation(ClientReservationId);

            // Assert
            var OkResult = Assert.IsType<OkObjectResult>(result);
            var Responce = OkResult.Value;
            Assert.NotNull(Responce);
        }

}