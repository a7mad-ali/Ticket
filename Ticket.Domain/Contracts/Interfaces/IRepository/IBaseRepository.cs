using System;
using System.Collections.Generic;
using System.Text;

namespace Ticket.Domain.Contracts.Interfaces.IRepository
{
    public  interface IBaseRepository<T> where T : class
    {
       public Task <T?> GetByIdAsync(int id);
       public Task<IReadOnlyList<T>> GetAllAsync();
   
       public Task AddAsync(T entity);
       public void Update(T entity);
       public void Remove(T entity);
   
       public Task SaveChangesAsync();
    }
}
