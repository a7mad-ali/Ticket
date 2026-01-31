using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Data
{
    public  class TicketDbContext :DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options):base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<TicketMessage> messages { get; set; }
        public DbSet<SupportTicket> Tickets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }


    }
}
