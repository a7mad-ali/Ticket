using AutoMapper;
using Ticket.Domain.Contracts.DTOs.Tickets;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Mappings.Profiles
{
    public class TicketsProfile : Profile
    {
        public TicketsProfile()
        {
            CreateMap<TicketAttachment, TicketAttachmentDto>()
                .ForCtorParam("AttachmentId", opt => opt.MapFrom(src => src.Id));
          
            CreateMap<TicketMessage, TicketMessageDto>()
                .ForCtorParam("MessageId", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Attachments", opt => opt.MapFrom(src => src.Attachments));

            
            CreateMap<SupportTicket, TicketDetailsDto>()
                .ForCtorParam("TicketId", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("DepartmentId", opt => opt.MapFrom(src => src.DepartmentId))
                .ForCtorParam("DepartmentName", opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ForCtorParam("Messages", opt => opt.MapFrom(src =>
                    src.Messages.OrderBy(m => m.CreatedAtUtc).ToList()
                ));
        }
    }
}
