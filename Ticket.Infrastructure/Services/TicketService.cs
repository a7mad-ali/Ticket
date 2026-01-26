using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ticket.Domain.Entities;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Contracts.DTOs.Tickets;

namespace Ticket.Infrastructure.Services
{
    // No interface implementation here
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public Task AddMessageAsync(Guid ticketId, AddMessageRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<TicketCreatedResponseDto> CreateTicketAsync(CreateTicketRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<TicketDetailsDto>> GetMyTicketsAsync(Guid userId)
        {
            var mine = await _ticketRepository.GetTicketsByUserIdAsync(userId);
            return mine.Select(MapToTicketDetailsDto).ToList();

        }

        public Task<TicketDetailsDto> GetTicketAsync(Guid ticketId)
        {
            throw new NotImplementedException();
        }
    }
}
