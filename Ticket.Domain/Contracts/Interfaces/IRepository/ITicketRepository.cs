using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public  interface ITicketRepository:IBaseRepository<SupportTicket>
    {
        Task<SupportTicket?> GetTicketWithMessagesAsync(Guid ticketId);
        Task<IReadOnlyList<SupportTicket>> GetTicketsByUserIdAsync(Guid userId);

    }
}
