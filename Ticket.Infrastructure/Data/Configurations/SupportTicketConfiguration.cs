using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data.Configurations
{
    public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.Property(x => x.Topic).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(20).IsRequired();

            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.LastUpdatedAtUtc).IsRequired();

            builder.HasOne(x => x.CreatedByUser)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
