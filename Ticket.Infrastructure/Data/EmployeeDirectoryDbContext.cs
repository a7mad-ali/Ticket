using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data
{
    public class EmployeeDirectoryDbContext : DbContext
    {
        public EmployeeDirectoryDbContext(DbContextOptions<EmployeeDirectoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeDirectoryEntry> EmployeeDirectoryEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.EmployeeDirectoryEntryConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
