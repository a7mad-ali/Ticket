using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticket.Domain.Entities;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Contracts.DTOs.Tickets;

namespace Ticket.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task AddMessageAsync(AddMessageRequestDto dto)
        {
            try
            {
                var ticket = await _ticketRepository.GetTicketWithMessagesAsync(dto.TicketId);
                if (ticket is null)
                {
                    throw new KeyNotFoundException($"Ticket {dto.TicketId} not found.");
                }

                var sender = await _userRepository.GetByIdAsync(dto.SenderUserId);
                if (sender is null)
                {
                    throw new InvalidOperationException("Sender user not found.");
                }

                ticket.Messages.Add(new TicketMessage
                {
                    TicketId = ticket.Id,
                    SenderUserId = dto.SenderUserId,
                    Body = dto.Body,
                    CreatedAtUtc = DateTime.UtcNow
                });

                ticket.LastUpdatedAtUtc = DateTime.UtcNow;

                _ticketRepository.Update(ticket);
                await _ticketRepository.SaveChangesAsync();
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to add ticket message.", ex);
            }
        }

        public async Task<TicketCreatedResponseDto> CreateTicketAsync(CreateTicketRequestDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user is null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                var ticket = new SupportTicket
                {
                    CreatedByUserId = dto.UserId,
                    Topic = dto.Topic,
                    Title = dto.Title,
                    Status = "Open",
                    CreatedAtUtc = DateTime.UtcNow,
                    LastUpdatedAtUtc = DateTime.UtcNow,
                    Messages = new List<TicketMessage>
                    {
                        new TicketMessage
                        {
                            SenderUserId = dto.UserId,
                            Body = dto.FirstMessage,
                            CreatedAtUtc = DateTime.UtcNow
                        }
                    }
                };

                await _ticketRepository.AddAsync(ticket);
                await _ticketRepository.SaveChangesAsync();

                return new TicketCreatedResponseDto(ticket.Id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to create ticket.", ex);
            }
        }

        public async Task<IReadOnlyList<TicketDetailsDto>> GetMyTicketsAsync(int userId)
        {
            try
            {
                var mine = await _ticketRepository.GetTicketsByUserIdAsync(userId);
                return mine.Select(ticket => _mapper.Map<TicketDetailsDto>(ticket)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve tickets.", ex);
            }
        }

        public async Task<TicketDetailsDto> GetTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _ticketRepository.GetTicketWithMessagesAsync(ticketId);
                if (ticket is null)
                {
                    throw new KeyNotFoundException($"Ticket {ticketId} not found.");
                }

                return _mapper.Map<TicketDetailsDto>(ticket);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve ticket details.", ex);
            }
        }
    }
}
