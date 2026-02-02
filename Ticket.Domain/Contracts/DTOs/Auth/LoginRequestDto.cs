namespace Ticket.Domain.Contracts.DTOs.Auth
{
    public record LoginRequestDto(
        string EmployeeCodeOrEmail,
        string Password
    );
}
