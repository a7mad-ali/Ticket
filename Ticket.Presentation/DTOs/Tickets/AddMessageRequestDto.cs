namespace Ticket.Presentation.DTOs.Tickets
{
    public record AddMessageRequestDto(
     Guid SenderUserId,
     string Body
 );
}
