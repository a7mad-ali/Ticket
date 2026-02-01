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
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly EmailOptions _emailOptions;

        public UserService(
            IUserRepository userRepository,
            IEmailSender emailSender,
            IOptions<EmailOptions> emailOptions)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _emailOptions = emailOptions.Value;
        }

        public async Task<UserPreCheckResponseDto> PreCheckAsync(UserPreCheckRequestDto dto)
        {
            try
            {
                var user = await _userRepository
                    .GetByEmployeeCodeAndNationalIdAsync(dto.EmployeeCode, dto.NationalId);

                if (user is null)
                {
                    return new UserPreCheckResponseDto("notFound", null, null, null, null);
                }

                var hasMissingDetails = string.IsNullOrWhiteSpace(user.FullName)
                    || string.IsNullOrWhiteSpace(user.Email)
                    || string.IsNullOrWhiteSpace(user.Phone)
                    || string.IsNullOrWhiteSpace(user.DepartmentName);

                if (hasMissingDetails)
                {
                    return new UserPreCheckResponseDto("needsRegistration", null, null, null, null);
                }

                return new UserPreCheckResponseDto(
                    "valid",
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.DepartmentName);
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
                if (string.IsNullOrWhiteSpace(dto.EmployeeCode)
                    || string.IsNullOrWhiteSpace(dto.NationalId))
                {
                    throw new InvalidOperationException("Employee code and national ID are required.");
                }

                var directoryByEmployeeCode = await _userRepository
                    .GetByEmployeeCodeAsync(dto.EmployeeCode);
                var directoryByNationalId = await _userRepository
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

                var resolvedFullName = string.IsNullOrWhiteSpace(dto.FullName)
                    ? directoryEntry.FullName
                    : dto.FullName;
                var resolvedEmail = string.IsNullOrWhiteSpace(dto.Email)
                    ? directoryEntry.Email
                    : dto.Email;
                var resolvedPhone = string.IsNullOrWhiteSpace(dto.Phone)
                    ? directoryEntry.Phone
                    : dto.Phone;
                var resolvedDepartment = string.IsNullOrWhiteSpace(dto.DepartmentName)
                    ? directoryEntry.DepartmentName
                    : dto.DepartmentName;

                if (string.IsNullOrWhiteSpace(resolvedFullName))
                {
                    throw new InvalidOperationException("Full name is required.");
                }

                if (string.IsNullOrWhiteSpace(resolvedEmail))
                {
                    throw new InvalidOperationException("Email is required.");
                }

                var existingEmail = await _userRepository.GetByEmailAsync(resolvedEmail);
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

                var user = new User
                {
                    EmployeeCode = dto.EmployeeCode,
                    NationalId = dto.NationalId,
                    FullName = resolvedFullName,
                    Email = resolvedEmail,
                    Phone = resolvedPhone ?? string.Empty,
                    DepartmentName = resolvedDepartment ?? string.Empty
                };
                user.IsEmailVerified = false;
                user.EmailVerificationCode = GenerateVerificationCode();
                user.EmailVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(10);
                user.CreatedAtUtc = DateTime.UtcNow;

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                try
                {
                    await SendVerificationEmailAsync(user);
                }
                catch (Exception ex)
                {
                    _userRepository.Remove(user);
                    await _userRepository.SaveChangesAsync();
                    throw new InvalidOperationException("Unable to send verification email.", ex);
                }

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
            var verificationCode = user.EmailVerificationCode ?? string.Empty;
            var verificationLink = string.Empty;
            if (!string.IsNullOrWhiteSpace(_emailOptions.VerificationBaseUrl))
            {
                verificationLink =
                    $"{_emailOptions.VerificationBaseUrl}?email={Uri.EscapeDataString(user.Email)}&code={Uri.EscapeDataString(verificationCode)}";
            }

            var subject = "Verify your email";
            var body = new StringBuilder()
                .Append("<div style=\"font-family:Arial,sans-serif;max-width:560px;margin:0 auto;padding:24px;border:1px solid #e5e7eb;border-radius:12px;\">")
                .Append("<h2 style=\"color:#111827;margin:0 0 8px;\">Ticket Support</h2>")
                .Append("<p style=\"color:#374151;margin:0 0 16px;\">Use the verification code below to finish creating your account.</p>")
                .Append("<div style=\"background:#f3f4f6;border-radius:10px;padding:16px;text-align:center;margin:16px 0;\">")
                .Append($"<span style=\"font-size:28px;letter-spacing:6px;font-weight:bold;color:#111827;\">{verificationCode}</span>")
                .Append("</div>")
                .Append("<p style=\"color:#6b7280;margin:0 0 16px;\">This code expires in 10 hours.</p>")
                .Append(string.IsNullOrWhiteSpace(verificationLink)
                    ? "<p style=\"color:#374151;margin:0 0 16px;\">Enter this code in the verification screen to activate your account.</p>"
                    : $"<p style=\"margin:0 0 16px;\"><a style=\"display:inline-block;background:#2563eb;color:#ffffff;padding:10px 18px;border-radius:8px;text-decoration:none;\" href=\"{verificationLink}\">Verify Email</a></p>")
                .Append("<p style=\"color:#9ca3af;margin:0;\">If you did not request this, please ignore this email.</p>")
                .Append("</div>")
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
