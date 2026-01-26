using AutoMapper;
using System;
using System.Threading.Tasks;
using Ticket.Domain.Contracts.DTOs.Users;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Contracts.Interfaces.IService;
using Ticket.Domain.Entities;

namespace Ticket.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserPreCheckResponseDto> PreCheckAsync(UserPreCheckRequestDto dto)
        {
            try
            {
                var byEmployeeCode = await _userRepository.GetByEmployeeCodeAsync(dto.EmployeeCode);
                if (byEmployeeCode is not null)
                {
                    return new UserPreCheckResponseDto(false, byEmployeeCode.FullName);
                }

                var byNationalId = await _userRepository.GetByNationalIdAsync(dto.NationalId);
                if (byNationalId is not null)
                {
                    return new UserPreCheckResponseDto(false, byNationalId.FullName);
                }

                return new UserPreCheckResponseDto(true, null);
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
                user.CreatedAtUtc = DateTime.UtcNow;

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

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

                user.IsEmailVerified = true;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                return new VerifyEmailResponseDto(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to verify email.", ex);
            }
        }
    }
}
