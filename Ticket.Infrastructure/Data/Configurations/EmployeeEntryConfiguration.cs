using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data.Configurations
{
    public class EmployeeEntryConfiguration : IEntityTypeConfiguration<EmployeeEntry>
    {
        public void Configure(EntityTypeBuilder<EmployeeEntry> builder)
        {
            builder.ToTable("EmployeeDirectory");

            builder.HasIndex(x => x.EmployeeCode).IsUnique();
            builder.HasIndex(x => x.NationalId).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            builder.Property(x => x.EmployeeCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.NationalId).HasMaxLength(50).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.Phone).HasMaxLength(50);
            builder.Property(x => x.DepartmentName).HasMaxLength(200);
        }
    }
}
