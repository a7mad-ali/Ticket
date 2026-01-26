using AutoMapper;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Mappings.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<RegisterUserRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
                .ForMember(dest => dest.IsEmailVerified, opt => opt.Ignore());
        }
    }
}
