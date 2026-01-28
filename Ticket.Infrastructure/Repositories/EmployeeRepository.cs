using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Entities;
using Ticket.Infrastructure.Data;

namespace Ticket.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeDbContext _context;

        public EmployeeRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public Task<EmployeeEntry?> GetByEmployeeCodeAsync(string employeeCode)
        {
            return _context.EmployeeDirectoryEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(E => E.EmployeeCode == employeeCode);
        }

        public Task<EmployeeEntry?> GetByNationalIdAsync(string nationalId)
        {
            return _context.EmployeeDirectoryEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(E => E.NationalId == nationalId);
        }
    }
}
