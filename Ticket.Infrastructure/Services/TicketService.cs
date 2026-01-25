using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ticket.Domain.Interfaces.IRepository;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Services
{
    // No interface implementation here
    public class TicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }
    }
}
