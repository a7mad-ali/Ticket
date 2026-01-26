using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ticket.Domain.Contracts.Interfaces.IRepository;
using Ticket.Infrastructure.Data;

namespace Ticket.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {

        protected readonly TicketDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(TicketDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
