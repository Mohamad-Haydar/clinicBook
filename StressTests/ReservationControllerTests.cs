using api.BusinessLogic.DataAccess.IDataAccess;
using api.BusinessLogic.DataAccess;
using api.Controllers;
using api.Data;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api.Models.Request;

namespace StressTests
{
    public class ReservationControllerTests
    {
        private IServiceProvider _serviceProvider;
        private ReservationController _controller;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            ConnectionStrings connectionStrings = new() { AppDbConnection = "User ID=mohamad;Password=#@!76Mohamad612;Host=localhost;Port=5432;Database=clinicbook;", IdentityDbConnection = "User ID=mohamad;Password=#@!76Mohamad612;Host=localhost;Port=5432;Database=clinicusers;" };
            ISqlDataAccess sqlDataAccess = new SqlDataAccess(); // Replace with real or mock implementation

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionStrings.AppDbConnection));

            services.AddDbContext<IdentityAppDbContext>(options =>
                options.UseNpgsql(connectionStrings.IdentityDbConnection));

            // Configure the identity services
            services.AddIdentity<UserModel, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityAppDbContext>()
                    .AddDefaultTokenProviders();

            services.AddScoped<ISqlDataAccess, SqlDataAccess>();
            services.AddScoped<IReservationData, ReservationData>();

            // Build the service provider
            _serviceProvider = services.BuildServiceProvider();

            // Resolve the services from the provider
            var reservationData = _serviceProvider.GetRequiredService<IReservationData>();

            // Create the controller instance with actual services
            _controller = new ReservationController(reservationData);
        }

        //[Benchmark]
        //public async Task GetReservationDetails_Test()
        //{
        //    await _controller.GetReservationDetails(13);
        //}

        [Benchmark]
        public async Task CreateQueueReservations_Test()
        {
            CreateQueueReservationRequest model = new CreateQueueReservationRequest()
            {
                client_id = "77d87d6d-ca66-4a6c-b51b-e558c6a5fb1e",
                doctor_availability_id = 63,
                doctor_service_ids = [1,2]
            };
            await _controller.CreateQueueReservation(model);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            // Clean up resources like the service provider if necessary
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
