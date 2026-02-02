namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketAttachmentRequestDto(
        string FileName,
        string ContentType,
        string FileUrl,
        long SizeInBytes
    );
}
