using api.BusinessLogic.DataAccess;
using api.BusinessLogic.DataAccess.IDataAccess;
using api.Controllers;
using api.Data;
using api.Helper;
using api.Internal.DataAccess;
using api.Models;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StressTests
{
    public class DoctorAvailabilityStressTests
    {
        private IServiceProvider _serviceProvider;
        private DoctorManagementController _controller;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            
            ConnectionStrings connectionStrings = new ConnectionStrings { AppDbConnection = "User ID=mohamad;Password=#@!76Mohamad612;Host=localhost;Port=5432;Database=clinicbook;", IdentityDbConnection = "User ID=mohamad;Password=#@!76Mohamad612;Host=localhost;Port=5432;Database=clinicusers;" };
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
            services.AddScoped<IDoctorManagementData, DoctorManagementData>();

            // Build the service provider
            _serviceProvider = services.BuildServiceProvider();

            // Resolve the services from the provider
            var doctorManagementData = _serviceProvider.GetRequiredService<IDoctorManagementData>();

            // Create the controller instance with actual services
            _controller = new DoctorManagementController(doctorManagementData);
        }

        // [Benchmark]
        // public async Task GetAllDoctors_Test()
        // {
        //     await _controller.GetAllDoctors(); // Invoke the controller method for benchmarking
        // }

        [Benchmark]
        public async Task GetADoctorById_Test()
        {
            await _controller.GetDoctorById("7999d2b1-cb78-4645-86ee-5a8e1a0850c5"); // Invoke the controller method for benchmarking
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