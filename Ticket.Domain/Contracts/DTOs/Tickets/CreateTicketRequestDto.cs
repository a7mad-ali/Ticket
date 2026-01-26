namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record CreateTicketRequestDto(
          Guid UserId,
          string Topic,
          string Title,
          string FirstMessage);

}
