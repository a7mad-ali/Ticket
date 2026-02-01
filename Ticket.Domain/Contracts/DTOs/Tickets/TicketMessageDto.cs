namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketMessageDto(
     int MessageId,
     int SenderUserId,
     string Body,
     DateTime CreatedAtUtc
 );
}
