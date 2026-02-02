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
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITicketNotifier _ticketNotifier;
        private readonly IMapper _mapper;

        public TicketService(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            ITicketNotifier ticketNotifier,
            IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _ticketNotifier = ticketNotifier;
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

                var message = new TicketMessage
                {
                    TicketId = ticket.Id,
                    SenderUserId = dto.SenderUserId,
                    Body = dto.Body,
                    CreatedAtUtc = DateTime.UtcNow,
                    Attachments = dto.Attachments?.Select(attachment => new TicketAttachment
                    {
                        TicketId = ticket.Id,
                        UploadedByUserId = dto.SenderUserId,
                        FileName = attachment.FileName,
                        ContentType = attachment.ContentType,
                        FileUrl = attachment.FileUrl,
                        SizeInBytes = attachment.SizeInBytes,
                        UploadedAtUtc = DateTime.UtcNow
                    }).ToList() ?? new List<TicketAttachment>()
                };

                foreach (var attachment in message.Attachments)
                {
                    attachment.Message = message;
                }

                ticket.Messages.Add(message);

                ticket.LastUpdatedAtUtc = DateTime.UtcNow;

                _ticketRepository.Update(ticket);
                await _ticketRepository.SaveChangesAsync();

                var messageDto = _mapper.Map<TicketMessageDto>(message);
                await _ticketNotifier.NotifyMessageAddedAsync(ticket.Id, messageDto);
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

                if (await _departmentRepository.GetByIdAsync(dto.DepartmentId) is null)
                {
                    throw new InvalidOperationException("Department not found.");
                }

                var firstMessage = new TicketMessage
                {
                    SenderUserId = dto.UserId,
                    Body = dto.FirstMessage,
                    CreatedAtUtc = DateTime.UtcNow,
                    Attachments = dto.Attachments?.Select(attachment => new TicketAttachment
                    {
                        UploadedByUserId = dto.UserId,
                        FileName = attachment.FileName,
                        ContentType = attachment.ContentType,
                        FileUrl = attachment.FileUrl,
                        SizeInBytes = attachment.SizeInBytes,
                        UploadedAtUtc = DateTime.UtcNow
                    }).ToList() ?? new List<TicketAttachment>()
                };

                var ticket = new SupportTicket
                {
                    CreatedByUserId = dto.UserId,
                    DepartmentId = dto.DepartmentId,
                    Topic = dto.Topic,
                    Title = dto.Title,
                    Status = "Open",
                    CreatedAtUtc = DateTime.UtcNow,
                    LastUpdatedAtUtc = DateTime.UtcNow,
                    Messages = new List<TicketMessage>
                    {
                        firstMessage
                    }
                };

                foreach (var attachment in firstMessage.Attachments)
                {
                    attachment.Ticket = ticket;
                    attachment.Message = firstMessage;
                }

                await _ticketRepository.AddAsync(ticket);
                await _ticketRepository.SaveChangesAsync();

                var createdTicketNotification = new TicketCreatedNotificationDto(
                    ticket.Id,
                    ticket.DepartmentId,
                    ticket.Topic,
                    ticket.Title,
                    ticket.CreatedAtUtc);
                await _ticketNotifier.NotifyTicketCreatedAsync(createdTicketNotification);

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
