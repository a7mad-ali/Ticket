namespace Ticket.Domain.Contracts.DTOs.Users
{
    public record UserPreCheckResponseDto(
        string Status,
        string? FullName,
        string? Email,
        string? Phone,
        string? DepartmentName
    );
}
