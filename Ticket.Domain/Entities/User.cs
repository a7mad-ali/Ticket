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
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public RegistrationStatus RegistrationStatus { get; set; } = RegistrationStatus.NotRegistered;
        public string? PasswordHash { get; set; }
        public string? EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeExpiresAtUtc { get; set; }
        public int EmailVerificationAttempts { get; set; }
        public DateTime? EmailVerificationLockedUntilUtc { get; set; }
        public int ResendCount { get; set; }
        public DateTime? LastResendAtUtc { get; set; }

        public string? Phone { get; set; }

        public int FailedLoginAttempts { get; set; }
        public DateTime? LoginLockedUntilUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }

}
