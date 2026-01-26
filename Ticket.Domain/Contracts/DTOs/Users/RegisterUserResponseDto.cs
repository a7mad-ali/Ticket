namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record RegisterUserResponseDto(
     Guid UserId,
     string Email
 );
}
