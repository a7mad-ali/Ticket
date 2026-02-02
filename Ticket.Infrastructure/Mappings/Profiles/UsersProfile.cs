using AutoMapper;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Mappings.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<RegistrationUpdateRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeCode, opt => opt.Ignore())
                .ForMember(dest => dest.NationalId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.IsEmailVerified, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationStatus, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerificationCode, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerificationCodeExpiresAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerificationAttempts, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerificationLockedUntilUtc, opt => opt.Ignore())
                .ForMember(dest => dest.ResendCount, opt => opt.Ignore())
                .ForMember(dest => dest.LastResendAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.FailedLoginAttempts, opt => opt.Ignore())
                .ForMember(dest => dest.LoginLockedUntilUtc, opt => opt.Ignore());
        }
    }
}
