using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data.Configurations
{
    public  class UserConfiguration : IEntityTypeConfiguration<User>

    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasIndex(x => x.EmployeeCode).IsUnique();
            builder.HasIndex(x => x.NationalId).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.EmployeeCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.NationalId).HasMaxLength(50).IsRequired();

            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DepartmentName).HasMaxLength(100).IsRequired();

            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(30).IsRequired();

            builder.Property(x => x.EmailVerificationCode)
                .HasMaxLength(20);
            builder.Property(x => x.EmailVerificationCodeExpiresAtUtc);

            builder.Property(x => x.CreatedAtUtc).IsRequired();
        }
    }
}
