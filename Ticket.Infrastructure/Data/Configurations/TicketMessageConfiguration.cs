using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data.Configurations
{
    public class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
    {
        public void Configure(EntityTypeBuilder<TicketMessage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Body).HasMaxLength(4000).IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();

            builder.HasOne(x => x.Ticket)
                   .WithMany(t => t.Messages)
                   .HasForeignKey(x => x.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SenderUser)
                   .WithMany()
                   .HasForeignKey(x => x.SenderUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
