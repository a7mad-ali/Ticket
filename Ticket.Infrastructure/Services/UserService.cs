using AutoMapper;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Entities;
using Ticket.Infrastructure.Repositories;

namespace Ticket.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly EmailOptions _emailOptions;
        private readonly IMapper _mapper;

        public UserService(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IEmailSender emailSender,
            IOptions<EmailOptions> emailOptions,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _emailOptions = emailOptions.Value;
            _mapper = mapper;
        }

        public async Task<UserPreCheckResponseDto> PreCheckAsync(UserPreCheckRequestDto dto)
        {
            try
            {
                var directoryByEmployeeCode = await _employeeRepository
                    .GetByEmployeeCodeAsync(dto.EmployeeCode);
                var directoryByNationalId = await _employeeRepository
                    .GetByNationalIdAsync(dto.NationalId);

                var directoryEntry = directoryByEmployeeCode ?? directoryByNationalId;
                if (directoryEntry is null)
                {
                    return new UserPreCheckResponseDto(false, null, null, null, null);
                }

                if (!string.Equals(directoryEntry.EmployeeCode, dto.EmployeeCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(directoryEntry.NationalId, dto.NationalId, StringComparison.OrdinalIgnoreCase))
                {
                    return new UserPreCheckResponseDto(false, null, null, null, null);
                }

                var byEmployeeCode = await _userRepository.GetByEmployeeCodeAsync(dto.EmployeeCode);
                if (byEmployeeCode is not null)
                {
                    return new UserPreCheckResponseDto(false, byEmployeeCode.FullName, null, null, null);
                }

                var byNationalId = await _userRepository.GetByNationalIdAsync(dto.NationalId);
                if (byNationalId is not null)
                {
                    return new UserPreCheckResponseDto(false, byNationalId.FullName, null, null, null);
                }

                return new UserPreCheckResponseDto(
                    true,
                    directoryEntry.FullName,
                    directoryEntry.Email,
                    directoryEntry.DepartmentName,
                    directoryEntry.Phone);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to pre-check user registration.", ex);
            }
        }

        public async Task<RegisterUserResponseDto> RegisterAsync(RegisterUserRequestDto dto)
        {
            try
            {
                var directoryByEmployeeCode = await _employeeRepository
                    .GetByEmployeeCodeAsync(dto.EmployeeCode);
                var directoryByNationalId = await _employeeRepository
                    .GetByNationalIdAsync(dto.NationalId);

                var directoryEntry = directoryByEmployeeCode ?? directoryByNationalId;
                if (directoryEntry is null)
                {
                    throw new InvalidOperationException("Employee record not found in directory.");
                }

                if (!string.Equals(directoryEntry.EmployeeCode, dto.EmployeeCode, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(directoryEntry.NationalId, dto.NationalId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Employee record does not match provided identifiers.");
                }

                var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
                if (existingEmail is not null)
                {
                    throw new InvalidOperationException("Email already registered.");
                }

                var existingEmployee = await _userRepository.GetByEmployeeCodeAsync(dto.EmployeeCode);
                if (existingEmployee is not null)
                {
                    throw new InvalidOperationException("Employee code already registered.");
                }

                var existingNationalId = await _userRepository.GetByNationalIdAsync(dto.NationalId);
                if (existingNationalId is not null)
                {
                    throw new InvalidOperationException("National ID already registered.");
                }

                var user = _mapper.Map<User>(dto);
                user.IsEmailVerified = false;
                user.EmailVerificationCode = GenerateVerificationCode();
                user.EmailVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddHours(10);
                user.CreatedAtUtc = DateTime.UtcNow;

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                await SendVerificationEmailAsync(user);

                return new RegisterUserResponseDto(user.Id, user.Email);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to register user.", ex);
            }
        }

        public async Task<VerifyEmailResponseDto> VerifyEmailAsync(VerifyEmailRequestDto dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user is null)
                {
                    return new VerifyEmailResponseDto(false);
                }

                if (string.IsNullOrWhiteSpace(user.EmailVerificationCode)
                    || user.EmailVerificationCodeExpiresAtUtc is null
                    || user.EmailVerificationCodeExpiresAtUtc < DateTime.UtcNow)
                {
                    return new VerifyEmailResponseDto(false);
                }

                if (!string.Equals(user.EmailVerificationCode, dto.VerificationCode, StringComparison.OrdinalIgnoreCase))
                {
                    return new VerifyEmailResponseDto(false);
                }

                user.IsEmailVerified = true;
                user.EmailVerificationCode = null;
                user.EmailVerificationCodeExpiresAtUtc = null;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new VerifyEmailResponseDto(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to verify email.", ex);
            }
        }

        private async Task SendVerificationEmailAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(_emailOptions.VerificationBaseUrl))
            {
                return;
            }

            var verificationLink =
                $"{_emailOptions.VerificationBaseUrl}?email={Uri.EscapeDataString(user.Email)}&code={Uri.EscapeDataString(user.EmailVerificationCode ?? string.Empty)}";
            var subject = "Verify your email";
            var body = new StringBuilder()
                .Append("<p>Please verify your email by clicking the link below:</p>")
                .Append($"<p><a href=\"{verificationLink}\">Verify Email</a></p>")
                .Append("<p>If you did not request this, please ignore this email.</p>")
                .ToString();

            await _emailSender.SendEmailAsync(user.Email, subject, body);
        }

        private static string GenerateVerificationCode()
        {
            Span<byte> data = stackalloc byte[4];
            RandomNumberGenerator.Fill(data);
            var value = BitConverter.ToUInt32(data);
            return (value % 1_000_000).ToString("D6");
        }
    }
}
