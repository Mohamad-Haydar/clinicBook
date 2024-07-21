using Xunit;
using Moq;
using api.Data;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;

namespace API.Controllers.Tests;

public class ReservationControllerTests
{
    private ReservationController _reservationController;
    private readonly ApplicationDbContext _appDbContext;
    private readonly IReservationData _reservationData;
    
    public ReservationControllerTests()
    {

        // _reservationController = new ReservationController();
    }

    [Fact]
    public async Task CreateQueueReservation_InvalidModel_ReturnsBadRequest()
    {
        // Arrange


        // Act

        // Assert
    }
}