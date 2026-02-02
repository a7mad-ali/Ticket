using Ticket.Domain.Contracts.DTOs.Auth;

namespace Ticket.Domain.Contracts.Interfaces.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
