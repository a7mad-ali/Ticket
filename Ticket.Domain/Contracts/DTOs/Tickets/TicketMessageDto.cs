namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketMessageDto(
     Guid MessageId,
     Guid SenderUserId,
     string Body,
     DateTime CreatedAtUtc
 );
}
