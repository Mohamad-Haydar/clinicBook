using api.BusinessLogic.DataAccess;
using api.Data;
using api.Exceptions;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using api.Models.Request;
using API.Tests.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests.BusinessLogic
{
    public class ReservationDataTests
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings = Substitute.For<IOptions<ConnectionStrings>>();
        private readonly ApplicationDbContext _appContext;
        private readonly ISqlDataAccess _sql = Substitute.For<ISqlDataAccess>();
        private readonly DbFactory _factory;
        private readonly ReservationData _sut;

        public ReservationDataTests()
        {
            _factory = new DbFactory();
            _appContext = _factory.CreateAppContext();
            _sut = new ReservationData(_sql, _connectionStrings, _appContext);
        }

        [Fact]
        public async Task CreateQueueReservationAsync_TrowExceptoin()
        {
            // Arrange
            CreateQueueReservationRequest model = new();
            ConnectionStrings myOptions = new()
            {
                AppDbConnection = "connectionstring",
                IdentityDbConnection = "connectoinstring"
            };
            _connectionStrings.Value.Returns(myOptions);
            _sql.SaveDataAsync<CreateQueueReservationRequest>(Arg.Any<string>(), Arg.Any<CreateQueueReservationRequest>(), Arg.Any<string>()).Throws(new Exception());

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.CreateQueueReservationAsync(model));

            // Assert
            Assert.IsType<Exception>(exception);
        }

        [Fact]
        public async Task MarkCompleteReservationAsync_NotFoundThrowError()
        {
            // Arrange
            int clientReservationId = 7;
            IEnumerable<ClientReservationModel> clientReservationModels = [
                    new()
                        {
                            StartTime = new TimeSpan(8),
                            EndTime = new TimeSpan(12),
                            DoctorAvailabilityId = 2,
                            IsDone = false,
                            ClientId = "id1"
                    },
                    new()
                        {
                            StartTime = new TimeSpan(8),
                            EndTime = new TimeSpan(12),
                            DoctorAvailabilityId = 2,
                            IsDone = false,
                            ClientId = "id2"
                    },
                    new()
                        {
                            StartTime = new TimeSpan(8),
                            EndTime = new TimeSpan(12),
                            DoctorAvailabilityId = 2,
                            IsDone = false,
                            ClientId = "id3"
                    },
                ];
            _appContext.AddRange(clientReservationModels);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.MarkCompleteReservationAsync(clientReservationId));

            // Assert
            Assert.IsType<UserNotFoundException>(exception);
            Assert.Equal("This client reservation was not found", exception.Message);
        }

        [Fact]
        public async Task MarkCompleteReservationAsync_MarkCompletedSuccessfully()
        {
            // Arrange
            int clientReservationId = 1;
            IEnumerable<ClientReservationModel> clientReservationModels = [
                    new()
                        {
                            StartTime = new TimeSpan(8),
                            EndTime = new TimeSpan(12),
                            DoctorAvailabilityId = 2,
                            IsDone = false,
                            ClientId = "id1"
                    },
                    new()
                        {
                            StartTime = new TimeSpan(8),
                            EndTime = new TimeSpan(12),
                            DoctorAvailabilityId = 2,
                            IsDone = false,
                            ClientId = "id2"
                    },
                    new()
                        {
                            StartTime = new TimeSpan(8),
                            EndTime = new TimeSpan(12),
                            DoctorAvailabilityId = 2,
                            IsDone = false,
                            ClientId = "id3"
                    },
                ];
            _appContext.AddRange(clientReservationModels);
            _appContext.SaveChanges();

            // Act
            var exception = await Record.ExceptionAsync(() => _sut.MarkCompleteReservationAsync(clientReservationId));
            var clientRes = await _appContext.ClientReservations.FirstOrDefaultAsync(x => x.Id == clientReservationId);

            // Assert
            Assert.Null(exception);
            Assert.NotNull(clientRes);
            Assert.True(clientRes.IsDone);
            Assert.Equal("id1",clientRes.ClientId);
        }


    }
}
