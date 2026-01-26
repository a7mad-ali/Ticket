using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Entities;
using Ticket.Infrastructure.Data;

namespace Ticket.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(TicketDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmployeeCodeAsync(string employeeCode)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.EmployeeCode == employeeCode);
        }

        public async Task<User?> GetByNationalIdAsync(string nationalId)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.NationalId == nationalId);
        }
    }
}
