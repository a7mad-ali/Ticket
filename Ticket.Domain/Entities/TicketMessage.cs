using System;
using System.Collections.Generic;
using System.Text;

namespace Ticket.Domain.Entities
{
    public  class TicketMessage
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public SupportTicket? Ticket { get; set; }

        public int SenderUserId { get; set; }
        public User? SenderUser { get; set; }

        public string Body { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<TicketAttachment> Attachments { get; set; } = new();
    }
}
