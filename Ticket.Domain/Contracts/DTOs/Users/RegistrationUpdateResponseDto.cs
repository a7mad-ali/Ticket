using System;

namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record RegistrationUpdateResponseDto(
        int UserId,
        string Status,
        string Email,
        DateTime? VerificationExpiresAtUtc
    );
}
