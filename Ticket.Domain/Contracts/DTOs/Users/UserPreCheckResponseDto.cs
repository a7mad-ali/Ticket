namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record UserPreCheckResponseDto(
        string NextStep,
        int? UserId,
        string Message
    );
}
