using System.Collections.Generic;

namespace Ticket.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int? ManagerUserId { get; set; }
        public User? ManagerUser { get; set; }

        public List<User> Users { get; set; } = new();
        public List<SupportTicket> Tickets { get; set; } = new();
    }
}
