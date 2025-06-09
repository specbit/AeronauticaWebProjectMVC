using FlyTickets2025.Data;
using FlyTickets2025.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.Web.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveAllAsync(); // Save immediately after create
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await SaveAllAsync(); // Save immediately after update
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id); // Get the entity by ID
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await SaveAllAsync(); // Save immediately after delete
            }
        }

        public async Task<bool> ExistsAsync(int id) 
        {
            return await _context.Set<T>().AnyAsync(e => e.Id == id);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
