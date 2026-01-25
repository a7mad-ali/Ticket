using System;
using System.Collections.Generic;
using System.Text;

namespace Ticket.Domain.Entities
{
    public class User
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string EmployeeCode { get; set; } = null!;
        public string NationalId { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;

        public string Email { get; set; } = null!;
        public bool IsEmailVerified { get; set; }

        public string Phone { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
