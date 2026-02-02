namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record CreateTicketRequestDto(
          int UserId,
          int DepartmentId,
          string Topic,
          string Title,
          string FirstMessage,
          List<TicketAttachmentRequestDto>? Attachments);

}
