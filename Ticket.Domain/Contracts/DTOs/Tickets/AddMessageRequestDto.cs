namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record AddMessageRequestDto(
     Guid SenderUserId,
     string Body
 );
}
