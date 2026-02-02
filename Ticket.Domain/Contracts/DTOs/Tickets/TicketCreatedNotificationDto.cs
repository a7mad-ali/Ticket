namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketCreatedNotificationDto(
        int TicketId,
        int DepartmentId,
        string Topic,
        string Title,
        DateTime CreatedAtUtc
    );
}
