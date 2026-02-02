namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record VerifyEmailRequestDto(
        string VerificationCode
    );
}
