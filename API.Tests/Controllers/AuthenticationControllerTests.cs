using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Models.Request;
using api.Models.Responce;
using Microsoft.AspNetCore.Http;
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

namespace API.Tests.Controllers
{
    public class AuthenticationControllerTests
    {
        private readonly IAuthenticationData _authenticationData = Substitute.For<IAuthenticationData>();
        private readonly AuthenticationController _sut;

        public AuthenticationControllerTests()
        {
            _sut = new AuthenticationController(_authenticationData);
        }

        [Fact]
        public void CreateUserRequest_InvalidModel_EmptyModel()
        {
            // Arrange
            CreateUserRequest model = new();
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("FirstName is required", errorLists);
            Assert.Contains("LastName is required", errorLists);
            Assert.Contains("Email is required", errorLists);
            Assert.Contains("Password is required", errorLists);
            Assert.Contains("Confirm Password is required", errorLists);
            Assert.Contains("PhoneNumber is required", errorLists);
        }

        [Fact]
        public void CreateUserRequest_InvalidModel_InvalidInput()
        {
            // Arrange
            CreateUserRequest model = new()
            {
                FirstName = "a",
                LastName = "abcdefghijklmnopqrstuvwxyz",
                Email = "randomEmail",
                Password = "randomPassword",
                ConfirmPassword = "random",
                PhoneNumber = "1234567890",
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
            Assert.Contains("First Name must be between 2 and 20 characters long", errorLists);
            Assert.Contains("Last Name must be between 2 and 20 characters long", errorLists);
            Assert.Contains("Invalid email address", errorLists);
            Assert.Contains("Passwords do not match", errorLists);
            Assert.Equal(4, errorLists.Count());
        }
    
    
        [Fact]
        public async Task RegisterClient_FailedLogic()
        {
            // Arrange
            CreateUserRequest model = new();
            _authenticationData.RegisterClientAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.RegisterClient(model);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseContent = badRequestResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Something went wrong. Please try again.", responseContent.Message);
        }

        [Fact]
        public async Task RegisterClient_RunsBusinessLogic()
        {
            // Arrange
            CreateUserRequest model = new();
            _authenticationData.RegisterClientAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.RegisterClient(model);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseContent = okResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Client created successfully. You can login to your account.", responseContent.Message);
        }
    
        [Fact]
        public void CreateSecretaryRequest_InvalidModel_EmptyModel()
        {
            // Arrange
            CreateSecretaryRequest model = new();
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("FirstName is required", errorLists);
            Assert.Contains("LastName is required", errorLists);
            Assert.Contains("Email is required", errorLists);
            Assert.Contains("Password is required", errorLists);
            Assert.Contains("Confirm Password is required", errorLists);
            Assert.Contains("PhoneNumber is required", errorLists);
        }

        [Fact]
        public void CreateSecretaryRequest_InvalidModel_InvalidInput()
        {
            // Arrange
            CreateSecretaryRequest model = new()
            {
                FirstName = "a",
                LastName = "abcdefghijklmnopqrstuvwxyz",
                Email = "randomEmail",
                Password = "randomPassword",
                ConfirmPassword = "random",
                PhoneNumber = "1234567890",
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
            Assert.Contains("First Name must be between 2 and 20 characters long", errorLists);
            Assert.Contains("Last Name must be between 2 and 20 characters long", errorLists);
            Assert.Contains("Invalid email address", errorLists);
            Assert.Contains("Passwords do not match", errorLists);
            Assert.Equal(4, errorLists.Count());
        }

        [Fact]
        public async Task RegisterSecretary_FailedLogic()
        {
            // Arrange
            CreateSecretaryRequest model = new();
            _authenticationData.RegisterSecretaryAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.RegisterSecretary(model);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseContent = badRequestResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Something went wrong. Please try again.", responseContent.Message);
        }

        [Fact]
        public async Task RegisterSecretary_RunsBusinessLogic()
        {
            // Arrange
            CreateSecretaryRequest model = new();
            _authenticationData.RegisterSecretaryAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.RegisterSecretary(model);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseContent = okResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Secretary created successfully. You can login to your account.", responseContent.Message);
        }

        [Fact]
        public void CreateDoctorRequest_InvalidModel_EmptyModel()
        {
            // Arrange
            CreateDoctorRequest model = new();
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("FirstName is required", errorLists);
            Assert.Contains("LastName is required", errorLists);
            Assert.Contains("Email is required", errorLists);
            Assert.Contains("Password is required", errorLists);
            Assert.Contains("Confirm Password is required", errorLists);
            Assert.Contains("PhoneNumber is required", errorLists);
            Assert.Contains("Description is required", errorLists);
            Assert.Contains("Number of people must be greater than 0", errorLists);
        }

        [Fact]
        public void CreateDoctorRequest_InvalidModel_InvalidInput()
        {
            // Arrange
            CreateDoctorRequest model = new()
            {
                FirstName = "a",
                LastName = "abcdefghijklmnopqrstuvwxyz",
                Email = "randomEmail",
                Password = "randomPassword",
                ConfirmPassword = "random",
                PhoneNumber = "1234567890",
                Description = "Description",
                CategoryId = 1
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
            Assert.Contains("First Name must be between 2 and 20 characters long", errorLists);
            Assert.Contains("Last Name must be between 2 and 20 characters long", errorLists);
            Assert.Contains("Invalid email address", errorLists);
            Assert.Contains("Passwords do not match", errorLists);
            Assert.Equal(4, errorLists.Count());
        }

        [Fact]
        public async Task RegisterDoctor_FailedLogic()
        {
            // Arrange
            CreateDoctorRequest model = new();
            _authenticationData.RegisterDoctorAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.RegisterDoctor(model);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseContent = badRequestResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Something went wrong. Please try again.", responseContent.Message);
        }

        [Fact]
        public async Task RegisterDoctor_RunsBusinessLogic()
        {
            // Arrange
            CreateDoctorRequest model = new();
            _authenticationData.RegisterDoctorAsync(model).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.RegisterDoctor(model);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseContent = okResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Doctor created successfully. You can login to your account.", responseContent.Message);
        }

        [Fact]
        public void LoginRequest_InvalidModel_EmptyModel()
        {
            // Arrange
            LoginRequest model = new();
            var context = new ValidationContext(model);
            var errors = new List<ValidationResult>();

            // Act
            var isModelValid = Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

            // Assert
            Assert.False(isModelValid);
            Assert.NotEmpty(errors);

            // check spesific validation errors
            var errorLists = errors.Select(x => x.ErrorMessage).ToList();
            Assert.Contains("Email is required", errorLists);
            Assert.Contains("Password is required", errorLists);
            Assert.Equal(2, errorLists.Count());
        }

        [Fact]
        public void LoginRequest_InvalidModel_InvalidInput()
        {
            // Arrange
            LoginRequest model = new()
            {
                Email = "randomEmail",
                Password = "randomPassword",
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
            Assert.Contains("Invalid email address", errorLists);
            Assert.Single(errorLists);
        }

        [Fact]
        public async Task LoginUser_FailedLogic()
        {
            // Arrange
            LoginRequest model = new();
            _authenticationData.LoginUserAsync(model).Throws(new Exception());

            // Act
            var result = await _sut.LoginUser(model);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseContent = badRequestResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Something went wrong. Please try again.", responseContent.Message);
        }

        [Fact]
        public async Task LoginUser_RunsBusinessLogic()
        {
            // Arrange
            LoginRequest model = new();
            AuthenticationResponse response = new()
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };
            _authenticationData.LoginUserAsync(model).Returns(Task.FromResult(response));

            var httpContext = new DefaultHttpContext();
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _sut.LoginUser(model);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseContent = okResult.Value as AuthenticationResponse;
            Assert.NotNull(responseContent);
            Assert.Equal("AccessToken", responseContent.AccessToken);
            Assert.Equal("RefreshToken", responseContent.RefreshToken);
        }

        [Fact]
        public async Task LogoutUser_FailedLogic()
        {
            // Arrange
            KeyValuePair<string, string> refreshPair = new();
            KeyValuePair<string, string> accessPair = new();
            _authenticationData.LogoutAsync(refreshPair, accessPair).Throws(new Exception());

            var httpContext = new DefaultHttpContext();
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _sut.Logout();

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseContent = badRequestResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Invalid client request", responseContent.Message);
        }

        [Fact]
        public async Task LogoutUser_RunsBusinessLogic()
        {
            // Arrange
            KeyValuePair<string, string> refreshPair = new();
            KeyValuePair<string, string> accessPair = new();
            _authenticationData.LogoutAsync(refreshPair, accessPair).Returns(Task.CompletedTask);

            var httpContext = new DefaultHttpContext();
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _sut.Logout();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseContent = okResult.Value as Response;
            Assert.NotNull(responseContent);
            Assert.Equal("Log out Successfully", responseContent.Message);
        }

    }
}
