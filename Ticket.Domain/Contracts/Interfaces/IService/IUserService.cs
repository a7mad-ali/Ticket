using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Contracts.DTOs.Users;

namespace Ticket.Domain.Contracts.Interfaces.IService
{
    public interface IUserService
    {
        Task<UserPreCheckResponseDto> PreCheckAsync(UserPreCheckRequestDto dto);
        Task<RegisterUserResponseDto> RegisterAsync(RegisterUserRequestDto dto);
        Task<VerifyEmailResponseDto> VerifyEmailAsync(VerifyEmailRequestDto dto);
    }
}
