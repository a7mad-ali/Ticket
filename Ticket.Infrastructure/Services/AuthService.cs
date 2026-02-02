using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ticket.Domain.Contracts.DTOs.Auth;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Entities;
using Ticket.Domain.Exceptions;

namespace Ticket.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeCodeOrEmail)
                || string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ApiException(400, "Employee code/email and password are required.", "INVALID_REQUEST");
            }

            var user = await _userRepository.GetByEmployeeCodeOrEmailAsync(dto.EmployeeCodeOrEmail.Trim());
            if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ApiException(401, "Invalid credentials.", "INVALID_CREDENTIALS");
            }

            var now = DateTime.UtcNow;
            if (user.LoginLockedUntilUtc is not null && user.LoginLockedUntilUtc > now)
            {
                throw new ApiException(423, "Login is locked. Try again later.", "LOGIN_LOCKED");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts += 1;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LoginLockedUntilUtc = now.AddMinutes(10);
                }

                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                throw new ApiException(401, "Invalid credentials.", "INVALID_CREDENTIALS");
            }

            if (!user.IsEmailVerified && user.RegistrationStatus != RegistrationStatus.Verified)
            {
                throw new ApiException(403, "Email is not verified.", "EMAIL_NOT_VERIFIED");
            }

            user.FailedLoginAttempts = 0;
            user.LoginLockedUntilUtc = null;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var token = GenerateToken(user, now, out var expiresAtUtc);

            return new LoginResponseDto(
                token,
                expiresAtUtc,
                "Bearer",
                null);
        }

        private string GenerateToken(User user, DateTime nowUtc, out DateTime expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(_jwtOptions.SigningKey))
            {
                throw new ApiException(500, "JWT signing key is missing.", "JWT_CONFIG");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("employeeCode", user.EmployeeCode),
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            expiresAtUtc = nowUtc.AddHours(_jwtOptions.AccessTokenHours);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: nowUtc,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
