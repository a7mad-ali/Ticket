namespace Ticket.Domain.Contracts.DTOs.Tickets
{
    public record TicketAttachmentDto(
        int AttachmentId,
        string FileName,
        string ContentType,
        string FileUrl,
        long SizeInBytes,
        DateTime UploadedAtUtc,
        int UploadedByUserId
    );
}
