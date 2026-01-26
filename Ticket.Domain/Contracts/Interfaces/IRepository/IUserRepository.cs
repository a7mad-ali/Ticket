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
    }
}
