using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using api.Controllers;
using api.BusinessLogic.DataAccess;
using api.Data;
using api.Models.Request;
using Microsoft.EntityFrameworkCore;
using api.Internal.DataAccess;
using Microsoft.Extensions.Options;
using api.Helper;

namespace API.Controllers.Tests;

public class ReservationControllerTests
{
    private readonly ApplicationDbContext _appDbContext;
    private readonly ReservationData _reservationData;
    private readonly ReservationController _controller;
    private readonly Mock<ISqlDataAccess> _sql;
    private readonly Mock<IOptions<ConnectionStrings>> _connectionStrings;

    public ReservationControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "clinicbook")
            .Options;

        _appDbContext = new ApplicationDbContext(options);
        _sql = new Mock<ISqlDataAccess>();
        _connectionStrings = new Mock<IOptions<ConnectionStrings>>();

        _reservationData = new ReservationData(_sql.Object, _connectionStrings.Object, _appDbContext);
        _controller = new ReservationController(_appDbContext, _reservationData);
    }

    [Fact]
    public async Task CreateQueueReservation_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await _controller.CreateQueueReservation(new CreateQueueReservationRequest());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task CreateQueueReservation_ValidModel_ReturnsOk()
    {
        // Arrange
        var model = new CreateQueueReservationRequest {  };

        // Act
        var result = await _controller.CreateQueueReservation(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    // [Fact]
    // public async Task CreateQueueReservation_ExceptionThrown_ReturnsBadRequest()
    // {
    //     // Arrange
    //     var model = new CreateQueueReservationRequest {
    //         client_id = "qwqw",
    //         doctor_availability_id = 125,
    //         // doctor_service_ids = [1,2]
    //     };

    //     // Act
    //     var result = await _controller.CreateQueueReservation(model);

    //     // Assert
    //     var badRequestResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.Equal(400, badRequestResult.StatusCode);
    // }
}