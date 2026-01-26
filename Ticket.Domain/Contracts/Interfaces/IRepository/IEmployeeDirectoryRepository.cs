using System.Threading.Tasks;
using Ticket.Domain.Entities;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public interface IEmployeeDirectoryRepository
    {
        Task<EmployeeDirectoryEntry?> GetByEmployeeCodeAsync(string employeeCode);
        Task<EmployeeDirectoryEntry?> GetByNationalIdAsync(string nationalId);
    }
}
