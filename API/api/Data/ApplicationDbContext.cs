using api.library.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClientModel> Client { get; set; }
        public DbSet<SecretaryModel> Secretary { get; set; }
        public DbSet<DoctorModel> DoctorModel {get; set;}
    }
}