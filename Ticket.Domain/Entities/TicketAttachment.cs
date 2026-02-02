using System;

namespace Ticket.Domain.Entities
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public SupportTicket? Ticket { get; set; }

        public int MessageId { get; set; }
        public TicketMessage? Message { get; set; }

        public int UploadedByUserId { get; set; }
        public User? UploadedByUser { get; set; }

        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public long SizeInBytes { get; set; }

        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
