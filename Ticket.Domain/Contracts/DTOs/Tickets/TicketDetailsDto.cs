namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketDetailsDto(
     int TicketId,
     string Topic,
     string Title,
     string Status,
     DateTime CreatedAtUtc,
     DateTime LastUpdatedAtUtc,
     List<TicketMessageDto> Messages
 );
}
