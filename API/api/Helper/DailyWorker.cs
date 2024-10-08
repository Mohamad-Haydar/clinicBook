﻿
using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace api.Helper
{
    public class DailyWorker : BackgroundService
    {
        private readonly ILogger<DailyWorker> _logger;
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public DailyWorker(ILogger<DailyWorker> logger, IOptions<ConnectionStrings> connectionStrings)
        {
            _logger = logger;
            _connectionStrings = connectionStrings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (IDbConnection connection = new NpgsqlConnection(_connectionStrings.Value.AppDbConnection))
                    {
                        await connection.ExecuteAsync("sp_delete_old_availability", new { }, commandType: CommandType.StoredProcedure);
                        await connection.ExecuteAsync("sp_open_automatic_availability", new { }, commandType: CommandType.StoredProcedure);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while Making daily checkup");
                }

                // Wait until the next scheduled time (e.g., next midnight)
                var nextRunTime = DateTime.Today.AddDays(1); // Schedule for midnight
                var delay = nextRunTime - DateTime.Now;

                _logger.LogInformation("Waiting until next run at: {time}", nextRunTime);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
