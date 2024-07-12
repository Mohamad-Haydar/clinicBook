using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClientModel> Clients { get; set; }
        public DbSet<SecretaryModel> Secretaries { get; set; }
        public DbSet<DoctorModel> Doctors {get; set;}
        public DbSet<DoctorServiceModel> DoctorServiceModels {get; set;}
        public DbSet<CategoryModel> CategoryModels {get; set;}
    }
}