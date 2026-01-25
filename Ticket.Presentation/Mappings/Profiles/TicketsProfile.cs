using AutoMapper;
using Ticket.Domain.Entities;
using Ticket.Presentation.DTOs.Tickets;

namespace Ticket.Presentation.Mappings.Profiles
{
    public class TicketsProfile : Profile
    {
        public TicketsProfile()
        {
            // Message Entity -> Message DTO
            CreateMap<TicketMessage, TicketMessageDto>()
                .ForCtorParam("MessageId", opt => opt.MapFrom(src => src.Id));

            // Ticket Entity -> TicketDetails DTO
            CreateMap<SupportTicket, TicketDetailsDto>()
                .ForCtorParam("TicketId", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Messages", opt => opt.MapFrom(src =>
                    src.Messages.OrderBy(m => m.CreatedAtUtc).ToList()
                ));
        }
    }
}
