namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record CreateTicketRequestDto(
          int UserId,
          string Topic,
          string Title,
          string FirstMessage);

}
