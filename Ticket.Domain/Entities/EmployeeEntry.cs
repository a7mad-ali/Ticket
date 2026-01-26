using System;

namespace Ticket.Domain.Entities
{
    public class EmployeeEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EmployeeCode { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; } 
        public string? Phone { get; set; }
        public string? DepartmentName { get; set; }
    }
}
