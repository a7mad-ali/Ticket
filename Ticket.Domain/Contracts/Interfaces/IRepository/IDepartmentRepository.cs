using Ticket.Domain.Entities;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<Department?> GetByIdAsync(int departmentId);
    }
}
