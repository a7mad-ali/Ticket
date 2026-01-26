namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record UserPreCheckResponseDto(
        bool IsValid,
        string? FullName
    );
}
