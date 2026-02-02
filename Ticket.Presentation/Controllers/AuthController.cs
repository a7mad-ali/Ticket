using Microsoft.AspNetCore.Mvc;
using Ticket.Domain.Contracts.DTOs.Auth;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Exceptions;

namespace Ticket.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(response);
            }
            catch (ApiException ex)
            {
                var message = ex.ErrorCode == "INVALID_CREDENTIALS"
                    ? "Invalid credentials."
                    : ex.Message;
                return StatusCode(ex.StatusCode, new { code = ex.ErrorCode, message });
            }
        }
    }
}
