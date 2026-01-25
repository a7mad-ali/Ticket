namespace Ticket.Presentation.DTOs.Users
{
    public record VerifyEmailRequestDto(
        string Email,
        string Code
    );
}
