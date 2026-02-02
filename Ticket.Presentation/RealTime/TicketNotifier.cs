using Microsoft.AspNetCore.SignalR;
using Ticket.Domain.Contracts.DTOs.Tickets;
using Ticket.Domain.Contracts.Interfaces.IService;

namespace Ticket.Presentation.RealTime
{
    public class TicketNotifier : ITicketNotifier
    {
        private readonly IHubContext<TicketHub> _hubContext;

        public TicketNotifier(IHubContext<TicketHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMessageAddedAsync(int ticketId, TicketMessageDto message)
        {
            return _hubContext.Clients.Group($"ticket-{ticketId}")
                .SendAsync("TicketMessageAdded", message);
        }

        public Task NotifyTicketCreatedAsync(TicketCreatedNotificationDto ticket)
        {
            return _hubContext.Clients.Group($"department-{ticket.DepartmentId}")
                .SendAsync("TicketCreated", ticket);
        }
    }
}
