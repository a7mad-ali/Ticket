using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data.Configurations
{
    public class EmployeeDirectoryEntryConfiguration : IEntityTypeConfiguration<EmployeeDirectoryEntry>
    {
        public void Configure(EntityTypeBuilder<EmployeeDirectoryEntry> builder)
        {
            builder.ToTable("EmployeeDirectory");

            builder.HasKey(entry => entry.Id);

            builder.Property(entry => entry.EmployeeCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(entry => entry.NationalId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(entry => entry.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(entry => entry.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(entry => entry.Phone)
                .HasMaxLength(50);

            builder.Property(entry => entry.DepartmentName)
                .HasMaxLength(200);

            builder.HasIndex(entry => entry.EmployeeCode).IsUnique();
            builder.HasIndex(entry => entry.NationalId).IsUnique();
            builder.HasIndex(entry => entry.Email).IsUnique();
        }
    }
}
