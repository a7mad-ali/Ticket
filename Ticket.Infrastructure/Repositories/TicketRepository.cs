using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;
using Ticket.Domain.Interfaces.IRepository;
using Ticket.Infrastructure.Data;

namespace Ticket.Infrastructure.Repositories
{
    public class TicketRepository : BaseRepository<SupportTicket>, ITicketRepository
    {
        public TicketRepository(TicketDbContext context)
       : base(context)
        {
        }

        public async Task<SupportTicket?> GetTicketWithMessagesAsync(Guid ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }
    }
}
