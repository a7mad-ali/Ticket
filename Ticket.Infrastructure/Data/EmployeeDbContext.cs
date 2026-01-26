using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeEntry> EmployeeDirectoryEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.EmployeeEntryConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
