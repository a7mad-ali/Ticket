namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record AddMessageRequestDto(
        Guid TicketId,
     Guid SenderUserId,
     string Body
 );
}
