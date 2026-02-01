namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record AddMessageRequestDto(
        int TicketId,
     int SenderUserId,
     string Body
 );
}
