using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Contracts.DTOs.Users;

namespace Ticket.Domain.Contracts.Interfaces.IService
{
    public interface IUserService
    {
        Task<UserPreCheckResponseDto> PreCheckAsync(UserPreCheckRequestDto dto);
        Task<RegistrationUpdateResponseDto> CompleteRegistrationAsync(int userId, RegistrationUpdateRequestDto dto);
        Task<VerifyEmailResponseDto> VerifyEmailAsync(int userId, VerifyEmailRequestDto dto);
        Task<RegistrationUpdateResponseDto> ResendVerificationEmailAsync(int userId);
    }
}
