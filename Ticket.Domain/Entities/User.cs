using System;
using System.Collections.Generic;
using System.Text;

namespace Ticket.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; } = null!;
        public string NationalId { get; set; } = null!;

        public string? FullName { get; set; }
        public string? DepartmentName { get; set; }

        public string? Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; } = RegistrationStatus.NotRegistered;
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeExpiresAtUtc { get; set; }
        public int EmailVerificationAttempts { get; set; }
        public DateTime? EmailVerificationLockedUntilUtc { get; set; }
        public int ResendCount { get; set; }
        public DateTime? LastResendAtUtc { get; set; }

        public string? Phone { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

}
