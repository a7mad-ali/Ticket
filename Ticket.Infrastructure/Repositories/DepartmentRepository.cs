using Microsoft.EntityFrameworkCore;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Domain.Entities;
using Ticket.Infrastructure.Data;

namespace Ticket.Infrastructure.Repositories
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(TicketDbContext context) : base(context)
        {
        }

        public async Task<Department?> GetByIdAsync(int departmentId)
        {
            return await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == departmentId);
        }
    }
}
