using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Entities;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public  interface IUserRepository :IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmployeeCodeAsync(string employeeCode);
        Task<User?> GetByEmployeeCodeOrEmailAsync(string employeeCodeOrEmail);
        Task<User?> GetByNationalIdAsync(string nationalId);
        Task<User?> GetByEmployeeCodeAndNationalIdAsync(string employeeCode, string nationalId);
        Task<User?> GetByPhoneAsync(string phone);
    }
}
