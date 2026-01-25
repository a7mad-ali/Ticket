namespace Ticket.Presentation.DTOs.Users
{
    public record UserPreCheckResponseDto(
        bool IsValid,
        string? FullName
    );
}
