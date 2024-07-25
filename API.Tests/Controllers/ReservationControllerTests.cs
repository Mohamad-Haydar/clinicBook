using Xunit;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Models.Request;
using NSubstitute;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Tests;

public class ReservationControllerTests
{
    private readonly ReservationController _sut;
    private readonly IReservationData  _reservationData = Substitute.For<IReservationData>();

    public ReservationControllerTests()
    {
        _sut = new ReservationController(_reservationData);
    }

    [Fact]
    public async Task CreateQueueReservation_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        CreateQueueReservationRequest model = new(){};
        _sut.ModelState.AddModelError("error","incomplete input");

        // Act
        var result = await _sut.CreateQueueReservation(model);

        // Assert   
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateQueueReservation_InvalidModel_ReturnsOK()
    {
        // Arrange
        CreateQueueReservationRequest model = new(){
            client_id = "ads",
            doctor_availability_id = 1
        };

        // Act
        var result = await _sut.CreateQueueReservation(model);

        // Assert   
        var badRequestResult = Assert.IsType<OkObjectResult>(result);
    }
}