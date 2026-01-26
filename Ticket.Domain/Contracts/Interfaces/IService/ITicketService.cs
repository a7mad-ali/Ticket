using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ticket.Domain.Entities;
using Ticket.Domain.Contracts.DTOs.Tickets;


namespace Ticket.Domain.Contracts.Interfaces.IService
{
    public interface ITicketService
    {
        Task<TicketCreatedResponseDto> CreateTicketAsync(CreateTicketRequestDto dto);
        Task AddMessageAsync(Guid ticketId, AddMessageRequestDto dto);
        Task<TicketDetailsDto> GetTicketAsync(Guid ticketId);
        Task<IReadOnlyList<TicketDetailsDto>> GetMyTicketsAsync(Guid userId);
    }
}
