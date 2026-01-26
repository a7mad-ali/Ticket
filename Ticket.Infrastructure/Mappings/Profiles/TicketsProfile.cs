using AutoMapper;
using Ticket.Domain.Contracts.DTOs.Tickets;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Mappings.Profiles
{
    public class TicketsProfile : Profile
    {
        public TicketsProfile()
        {
          
            CreateMap<TicketMessage, TicketMessageDto>()
                .ForCtorParam("MessageId", opt => opt.MapFrom(src => src.Id));

            
            CreateMap<SupportTicket, TicketDetailsDto>()
                .ForCtorParam("TicketId", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Messages", opt => opt.MapFrom(src =>
                    src.Messages.OrderBy(m => m.CreatedAtUtc).ToList()
                ));
        }
    }
}
