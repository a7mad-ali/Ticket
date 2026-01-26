using AutoMapper;
using System;
using System.Threading.Tasks;
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
        private readonly IMapper _mapper;

        public UserService(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
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
