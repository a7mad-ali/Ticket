using System;
using System.Collections.Generic;
using System.Text;

namespace Ticket.Domain.Entities
{
    public  class TicketMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TicketId { get; set; }
        public SupportTicket? Ticket { get; set; }

        public Guid SenderUserId { get; set; }
        public User? SenderUser { get; set; }

        public string Body { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
