using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PTracking.Models;

namespace PTracking.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SalesEntity> SalesData { get; set; }

        public DbSet<PTracking.Models.Tickets> Tickets { get; set; } = default!;

        public DbSet<PTracking.Models.Employee> Employee { get; set; } = default!;

        public DbSet<PTracking.Models.Project> Project { get; set; } = default!;

        public DbSet<PTracking.Models.Notification> Notification { get; set; } = default!;
        
    }
}