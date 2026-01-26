namespace Ticket.Domain.Contracts.DTOs.Users
{
public record VerifyEmailRequestDto(
        string Email,
        string VerificationCode
    );
}
