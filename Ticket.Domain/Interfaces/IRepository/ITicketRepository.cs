using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Domain.Interfaces.IRepository
{
    public  interface ITicketRepository:IBaseRepository<SupportTicket>
    {
        Task<SupportTicket?> GetTicketWithMessagesAsync(Guid ticketId);

    }
}
