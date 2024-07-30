using api.Helper;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace api.Services
{
    public class BackupService : IBackupService
    {
        private readonly IOptions<ConnectionStrings> _connectionStrings;

        public BackupService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task CreateBackupAsync()
        {
            string backupFolder = "Backup";
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
            string backupFileName1 = Path.Combine(backupFolder, $"db1.sql");
            string backupFileName2 = Path.Combine(backupFolder, $"db2.sql");

            DatabaseDetails db1 = new("clinicbook");
            DatabaseDetails db2 = new("clinicusers");

            try
            {
                await BackupDatabaseAsync(db1, backupFileName1);
                await BackupDatabaseAsync(db2, backupFileName2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CreateRestoreAsync ()
        {
            string backupFolder = "Backup";
            if (!Directory.Exists(backupFolder))
            {
                throw new Exception();
            }
            string backupFileName1 = Path.Combine(backupFolder, $"db1.sql");
            string backupFileName2 = Path.Combine(backupFolder, $"db2.sql");

            DatabaseDetails db1 = new("clinicbook");
            DatabaseDetails db2 = new("clinicusers");

            try
            {
                await RestoreDataBaseAsync(db1, backupFileName1);
                await RestoreDataBaseAsync(db2, backupFileName2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task BackupDatabaseAsync(DatabaseDetails dbDetails, string backupFileName)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "pg_dump",
                Arguments = $"-d {dbDetails.Name} -h {dbDetails.Host} -U {dbDetails.Username} -Fc -f {backupFileName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Environment =
                {
                    ["PGPASSWORD"] = dbDetails.Password
                }
            };


            var process = new Process
            {
                StartInfo = processInfo
            };

            try
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task RestoreDataBaseAsync(DatabaseDetails dbDetails, string backupFileName)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "pg_restore",
                Arguments = $"-d {dbDetails.Name} -h {dbDetails.Host} -U {dbDetails.Username} -c {backupFileName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Environment =
                {
                    ["PGPASSWORD"] = dbDetails.Password
                }
            };


            var process = new Process
            {
                StartInfo = processInfo
            };

            try
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class DatabaseDetails(string name)
    {
        public string Name { get; set; } = name;
        public string Host { get; set; } = "localhost";
        public string Username { get; set; } = "mohamad";
        public string Password { get; set; } = "#@!76Mohamad612";
    }
}
