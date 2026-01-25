using System;
using System.Collections.Generic;
using System.Text;

namespace Ticket.Domain.Entities
{
    public  class SupportTicket
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public string Topic { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Status { get; set; } = "Open";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<TicketMessage> Messages { get; set; } = new();
    }
}
