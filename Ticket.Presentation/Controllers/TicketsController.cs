using Microsoft.AspNetCore.Mvc;
using Ticket.Domain.Contracts.DTOs.Tickets;
using Ticket.Domain.Contracts.Interfaces.IService;

namespace Ticket.Presentation.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<ActionResult<TicketCreatedResponseDto>> CreateTicket(CreateTicketRequestDto dto)
        {
            try
            {
                var created = await _ticketService.CreateTicketAsync(dto);
                return CreatedAtAction(nameof(GetTicket), new { ticketId = created.TicketId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{ticketId:int}/messages")]
        public async Task<ActionResult<TicketMessageDto>> AddMessage(AddMessageRequestDto dto)
        {
            try
            {
                await _ticketService.AddMessageAsync( dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{ticketId:int}")]
        public async Task<ActionResult<TicketDetailsDto>> GetTicket(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.GetTicketAsync(ticketId);
                return Ok(ticket);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IReadOnlyList<TicketDetailsDto>>> GetTicketsForUser(int userId)
        {
            var tickets = await _ticketService.GetMyTicketsAsync(userId);
            return Ok(tickets);
        }
    }
}
