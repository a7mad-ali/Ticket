using Microsoft.AspNetCore.Mvc;
using System;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Exceptions;

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
            if (string.IsNullOrWhiteSpace(dto.EmployeeCode)
                || string.IsNullOrWhiteSpace(dto.NationalId)
                || string.Equals(dto.EmployeeCode, "0", StringComparison.OrdinalIgnoreCase)
                || string.Equals(dto.NationalId, "0", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Employee code and national ID are required and cannot be 0." });
            }

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
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
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
                return BadRequest(new { message = "Invalid or expired verification code." });
            }

            return Ok(response);
        }
    }
}
