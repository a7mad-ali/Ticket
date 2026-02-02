namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketDetailsDto(
     int TicketId,
     int DepartmentId,
     string DepartmentName,
     string Topic,
     string Title,
     string Status,
     DateTime CreatedAtUtc,
     DateTime LastUpdatedAtUtc,
     List<TicketMessageDto> Messages
 );
}
