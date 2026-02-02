using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Entities;
using Ticket.Domain.Exceptions;

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
                    return new UserPreCheckResponseDto("notFound", null, null, null, null, null);
                }

                var hasMissingDetails = string.IsNullOrWhiteSpace(user.FullName)
                    || string.IsNullOrWhiteSpace(user.Email)
                    || string.IsNullOrWhiteSpace(user.Phone)
                    || string.IsNullOrWhiteSpace(user.DepartmentName);

                var isVerified = user.IsEmailVerified || user.RegistrationStatus == RegistrationStatus.Verified;
                var needsRegistration = hasMissingDetails || !isVerified;

                return new UserPreCheckResponseDto(
                    needsRegistration ? "needsRegistration" : "valid",
                    user.Id,
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

        public async Task<RegistrationUpdateResponseDto> CompleteRegistrationAsync(
            int userId,
            RegistrationUpdateRequestDto dto)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userId);

                if (existingUser is null)
                {
                    throw new ApiException(404, "Employee record not found.", "USER_NOT_FOUND");
                }

                if (existingUser.IsEmailVerified || existingUser.RegistrationStatus == RegistrationStatus.Verified)
                {
                    throw new ApiException(409, "User already verified.", "ALREADY_VERIFIED");
                }

                var now = DateTime.UtcNow;
                if (existingUser.RegistrationStatus == RegistrationStatus.Locked
                    && existingUser.EmailVerificationLockedUntilUtc is not null
                    && existingUser.EmailVerificationLockedUntilUtc > now)
                {
                    throw new ApiException(423, "Verification is locked. Try again later.", "OTP_LOCKED");
                }

                var resolvedFullName = string.IsNullOrWhiteSpace(dto.FullName)
                    ? existingUser.FullName
                    : dto.FullName?.Trim();
                var resolvedEmail = string.IsNullOrWhiteSpace(dto.Email)
                    ? existingUser.Email
                    : dto.Email?.Trim();
                var resolvedPhone = string.IsNullOrWhiteSpace(dto.Phone)
                    ? existingUser.Phone
                    : dto.Phone?.Trim();
                var resolvedDepartment = string.IsNullOrWhiteSpace(dto.DepartmentName)
                    ? existingUser.DepartmentName
                    : dto.DepartmentName?.Trim();

                if (string.IsNullOrWhiteSpace(resolvedFullName))
                {
                    throw new ApiException(400, "Full name is required.", "FULL_NAME_REQUIRED");
                }

                if (string.IsNullOrWhiteSpace(resolvedEmail))
                {
                    throw new ApiException(400, "Email is required.", "EMAIL_REQUIRED");
                }

                if (string.IsNullOrWhiteSpace(resolvedPhone))
                {
                    throw new ApiException(400, "Phone is required.", "PHONE_REQUIRED");
                }

                if (string.IsNullOrWhiteSpace(resolvedDepartment))
                {
                    throw new ApiException(400, "Department name is required.", "DEPARTMENT_REQUIRED");
                }

                var existingEmail = await _userRepository.GetByEmailAsync(resolvedEmail);
                if (existingEmail is not null && existingEmail.Id != existingUser.Id)
                {
                    throw new ApiException(409, "Email already registered.", "EMAIL_IN_USE");
                }

                existingUser.FullName = resolvedFullName;
                existingUser.Email = resolvedEmail;
                existingUser.Phone = resolvedPhone ?? string.Empty;
                existingUser.DepartmentName = resolvedDepartment ?? string.Empty;
                existingUser.IsEmailVerified = false;
                if (existingUser.RegistrationStatus == RegistrationStatus.Locked
                    && existingUser.EmailVerificationLockedUntilUtc <= now)
                {
                    existingUser.EmailVerificationLockedUntilUtc = null;
                    existingUser.EmailVerificationAttempts = 0;
                    existingUser.RegistrationStatus = RegistrationStatus.PendingVerification;
                }

                var hasActiveCode = existingUser.RegistrationStatus == RegistrationStatus.PendingVerification
                    && !string.IsNullOrWhiteSpace(existingUser.EmailVerificationCode)
                    && existingUser.EmailVerificationCodeExpiresAtUtc > now;

                if (!hasActiveCode || dto.Resend)
                {
                    if (dto.Resend)
                    {
                        EnforceResendLimits(existingUser, now);
                    }
                    else
                    {
                        existingUser.ResendCount = 0;
                        existingUser.LastResendAtUtc = null;
                    }

                    existingUser.EmailVerificationCode = GenerateVerificationCode();
                    existingUser.EmailVerificationCodeExpiresAtUtc = now.AddMinutes(10);
                    existingUser.EmailVerificationAttempts = 0;
                    existingUser.EmailVerificationLockedUntilUtc = null;
                    existingUser.RegistrationStatus = RegistrationStatus.PendingVerification;
                }

                _userRepository.Update(existingUser);
                await _userRepository.SaveChangesAsync();

                if (!hasActiveCode || dto.Resend)
                {
                    try
                    {
                        await SendVerificationEmailAsync(existingUser);
                    }
                    catch (Exception)
                    {
                        throw new ApiException(400, "Unable to send verification email.", "EMAIL_SEND_FAILED");
                    }
                }

                return new RegistrationUpdateResponseDto(
                    existingUser.Id,
                    existingUser.RegistrationStatus.ToString(),
                    existingUser.Email ?? string.Empty,
                    existingUser.EmailVerificationCodeExpiresAtUtc);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new ApiException(400, "Unable to register user.", "REGISTRATION_FAILED");
            }
        }

        public async Task<VerifyEmailResponseDto> VerifyEmailAsync(int userId, VerifyEmailRequestDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user is null)
                {
                    throw new ApiException(404, "User not found.", "USER_NOT_FOUND");
                }

                if (user.IsEmailVerified || user.RegistrationStatus == RegistrationStatus.Verified)
                {
                    return new VerifyEmailResponseDto(true, null);
                }

                var now = DateTime.UtcNow;
                if (user.EmailVerificationLockedUntilUtc is not null
                    && user.EmailVerificationLockedUntilUtc > now)
                {
                    return new VerifyEmailResponseDto(false, "OTP_LOCKED");
                }

                var clearedLock = false;
                if (user.RegistrationStatus == RegistrationStatus.Locked
                    && user.EmailVerificationLockedUntilUtc <= now)
                {
                    user.RegistrationStatus = RegistrationStatus.PendingVerification;
                    user.EmailVerificationAttempts = 0;
                    user.EmailVerificationLockedUntilUtc = null;
                    clearedLock = true;
                }

                if (string.IsNullOrWhiteSpace(user.EmailVerificationCode)
                    || user.EmailVerificationCodeExpiresAtUtc is null
                    || user.EmailVerificationCodeExpiresAtUtc < now)
                {
                    if (clearedLock)
                    {
                        _userRepository.Update(user);
                        await _userRepository.SaveChangesAsync();
                    }
                    return new VerifyEmailResponseDto(false, "OTP_EXPIRED");
                }

                if (!IsVerificationCodeMatch(user.EmailVerificationCode, dto.VerificationCode))
                {
                    user.EmailVerificationAttempts += 1;
                    if (user.EmailVerificationAttempts >= 5)
                    {
                        user.RegistrationStatus = RegistrationStatus.Locked;
                        user.EmailVerificationLockedUntilUtc = now.AddMinutes(10);
                        _userRepository.Update(user);
                        await _userRepository.SaveChangesAsync();
                        return new VerifyEmailResponseDto(false, "OTP_LOCKED");
                    }

                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                    return new VerifyEmailResponseDto(false, "OTP_INVALID");
                }

                user.IsEmailVerified = true;
                user.RegistrationStatus = RegistrationStatus.Verified;
                user.EmailVerificationCode = null;
                user.EmailVerificationCodeExpiresAtUtc = null;
                user.EmailVerificationAttempts = 0;
                user.EmailVerificationLockedUntilUtc = null;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new VerifyEmailResponseDto(true, null);
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
                    $"{_emailOptions.VerificationBaseUrl}?userId={user.Id}&code={Uri.EscapeDataString(verificationCode)}";
            }

            var subject = "Verify your email";
            var body = new StringBuilder()
                .Append("<div style=\"font-family:Arial,sans-serif;max-width:560px;margin:0 auto;padding:24px;border:1px solid #e5e7eb;border-radius:12px;\">")
                .Append("<h2 style=\"color:#111827;margin:0 0 8px;\">Ticket Support</h2>")
                .Append("<p style=\"color:#374151;margin:0 0 16px;\">Use the verification code below to finish creating your account.</p>")
                .Append("<div style=\"background:#f3f4f6;border-radius:10px;padding:16px;text-align:center;margin:16px 0;\">")
                .Append($"<span style=\"font-size:28px;letter-spacing:6px;font-weight:bold;color:#111827;\">{verificationCode}</span>")
                .Append("</div>")
                .Append("<p style=\"color:#6b7280;margin:0 0 16px;\">This code expires in 10 minutes.</p>")
                .Append(string.IsNullOrWhiteSpace(verificationLink)
                    ? "<p style=\"color:#374151;margin:0 0 16px;\">Enter this code in the verification screen to activate your account.</p>"
                    : $"<p style=\"margin:0 0 16px;\"><a style=\"display:inline-block;background:#2563eb;color:#ffffff;padding:10px 18px;border-radius:8px;text-decoration:none;\" href=\"{verificationLink}\">Verify Email</a></p>")
                .Append("<p style=\"color:#9ca3af;margin:0;\">If you did not request this, please ignore this email.</p>")
                .Append("</div>")
                .ToString();

            await _emailSender.SendEmailAsync(user.Email, subject, body);
        }

        private static void EnforceResendLimits(User user, DateTime now)
        {
            if (user.LastResendAtUtc is null || now - user.LastResendAtUtc > TimeSpan.FromMinutes(15))
            {
                user.ResendCount = 0;
            }

            if (user.ResendCount >= 3)
            {
                throw new ApiException(429, "Resend limit reached. Try again later.", "RESEND_LIMIT");
            }

            user.ResendCount += 1;
            user.LastResendAtUtc = now;
        }

        private static string GenerateVerificationCode()
        {
            Span<byte> data = stackalloc byte[4];
            RandomNumberGenerator.Fill(data);
            var value = BitConverter.ToUInt32(data);
            return (value % 1_000_000).ToString("D6");
        }

        private static bool IsVerificationCodeMatch(string expected, string? provided)
        {
            if (string.IsNullOrWhiteSpace(provided))
            {
                return false;
            }

            var expectedBytes = Encoding.UTF8.GetBytes(expected);
            var providedBytes = Encoding.UTF8.GetBytes(provided.Trim());
            var maxLength = Math.Max(expectedBytes.Length, providedBytes.Length);
            var paddedExpected = new byte[maxLength];
            var paddedProvided = new byte[maxLength];
            Array.Copy(expectedBytes, paddedExpected, expectedBytes.Length);
            Array.Copy(providedBytes, paddedProvided, providedBytes.Length);

            var matches = CryptographicOperations.FixedTimeEquals(paddedExpected, paddedProvided);
            return matches && expectedBytes.Length == providedBytes.Length;
        }
    }
}
