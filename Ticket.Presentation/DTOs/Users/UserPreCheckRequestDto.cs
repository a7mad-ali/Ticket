namespace Ticket.Presentation.DTOs.Users
{

    public record UserPreCheckRequestDto(
        string EmployeeCode,
        string NationalId
    );
}
