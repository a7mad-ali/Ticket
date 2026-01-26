using System.Threading.Tasks;
using Ticket.Domain.Entities;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public interface IEmployeeRepository
    {
        Task<EmployeeEntry?> GetByEmployeeCodeAsync(string employeeCode);
        Task<EmployeeEntry?> GetByNationalIdAsync(string nationalId);
    }
}
