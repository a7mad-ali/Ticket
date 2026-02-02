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

        [HttpPut("{userId:int}/registration")]
        public async Task<ActionResult<RegistrationUpdateResponseDto>> CompleteRegistration(
            int userId,
            RegistrationUpdateRequestDto dto)
        {
            try
            {
                var response = await _userService.CompleteRegistrationAsync(userId, dto);
                return Ok(response);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { code = ex.ErrorCode, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{userId:int}/verify-email")]
        public async Task<ActionResult<VerifyEmailResponseDto>> VerifyEmail(
            int userId,
            VerifyEmailRequestDto dto)
        {
            try
            {
                var response = await _userService.VerifyEmailAsync(userId, dto);
                if (!response.Verified)
                {
                    var message = response.ErrorCode switch
                    {
                        "OTP_LOCKED" => "Verification is locked. Try again later.",
                        "OTP_EXPIRED" => "Verification code has expired.",
                        "OTP_INVALID" => "Invalid verification code.",
                        _ => "Invalid or expired verification code."
                    };
                    return BadRequest(new { code = response.ErrorCode, message });
                }

                return Ok(response);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, new { code = ex.ErrorCode, message = ex.Message });
            }
        }
    }
}
