using FlyTickets2025.Web.Data.Entities;

namespace FlyTickets2025.Web.Repositories
{
    public interface IGenericRepository<T> where T : class, IEntity // IEntity is a marker interface for entities
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id); // Changed to T? for nullability
        //Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> SaveAllAsync(); 
    }
}
