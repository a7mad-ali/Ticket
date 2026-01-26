using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Entities;
using Ticket.Infrastructure.Data;

namespace Ticket.Infrastructure.Repositories
{
    public class EmployeeDirectoryRepository : IEmployeeDirectoryRepository
    {
        private readonly EmployeeDirectoryDbContext _context;

        public EmployeeDirectoryRepository(EmployeeDirectoryDbContext context)
        {
            _context = context;
        }

        public Task<EmployeeDirectoryEntry?> GetByEmployeeCodeAsync(string employeeCode)
        {
            return _context.EmployeeDirectoryEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.EmployeeCode == employeeCode);
        }

        public Task<EmployeeDirectoryEntry?> GetByNationalIdAsync(string nationalId)
        {
            return _context.EmployeeDirectoryEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.NationalId == nationalId);
        }
    }
}
