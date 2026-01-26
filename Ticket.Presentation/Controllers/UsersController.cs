using Microsoft.AspNetCore.Mvc;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Contracts.Interfaces.IService;

namespace Ticket.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("precheck")]
        public async Task<ActionResult<UserPreCheckResponseDto>> PreCheck(UserPreCheckRequestDto dto)
        {
            var result = await _userService.PreCheckAsync(dto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponseDto>> Register(RegisterUserRequestDto dto)
        {
            try
            {
                var response = await _userService.RegisterAsync(dto);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult<VerifyEmailResponseDto>> VerifyEmail(VerifyEmailRequestDto dto)
        {
            var response = await _userService.VerifyEmailAsync(dto);
            if (!response.Verified)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(response);
        }
    }
}
