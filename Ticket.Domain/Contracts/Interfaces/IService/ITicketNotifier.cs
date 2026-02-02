using System.Threading.Tasks;
using Ticket.Domain.Contracts.DTOs.Tickets;

namespace Ticket.Domain.Contracts.Interfaces.IService
{
    public interface ITicketNotifier
    {
        Task NotifyMessageAddedAsync(int ticketId, TicketMessageDto message);
        Task NotifyTicketCreatedAsync(TicketCreatedNotificationDto ticket);
    }
}
