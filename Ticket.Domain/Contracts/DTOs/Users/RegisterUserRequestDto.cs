namespace Ticket.Domain.Contracts.DTOs.Users
{
public record RegisterUserRequestDto(
    string EmployeeCode,
    string NationalId,
    string? FullName,
    string? Email,
    string? Phone,
    string? DepartmentName
);
}
