namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record RegistrationUpdateRequestDto(
        string? FullName,
        string? Email,
        string? Phone,
        string? DepartmentName,
        bool Resend
    );
}
