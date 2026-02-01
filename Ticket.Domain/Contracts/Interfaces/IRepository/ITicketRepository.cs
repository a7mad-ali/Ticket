using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public  interface ITicketRepository:IBaseRepository<SupportTicket>
    {
        Task<SupportTicket?> GetTicketWithMessagesAsync(int ticketId);
        Task<IReadOnlyList<SupportTicket>> GetTicketsByUserIdAsync(int userId);

    }
}
