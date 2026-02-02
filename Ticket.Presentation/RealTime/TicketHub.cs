using Microsoft.AspNetCore.SignalR;

namespace Ticket.Presentation.RealTime
{
    public class TicketHub : Hub
    {
        public Task JoinTicketAsync(int ticketId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
        }

        public Task LeaveTicketAsync(int ticketId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
        }

        public Task JoinDepartmentAsync(int departmentId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"department-{departmentId}");
        }

        public Task LeaveDepartmentAsync(int departmentId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"department-{departmentId}");
        }
    }
}
