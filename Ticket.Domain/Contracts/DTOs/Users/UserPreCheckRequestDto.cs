namespace Ticket.Domain.Contracts.DTOs.Users
{

    public record UserPreCheckRequestDto(
        string EmployeeCode,
        string NationalId
    );
}
