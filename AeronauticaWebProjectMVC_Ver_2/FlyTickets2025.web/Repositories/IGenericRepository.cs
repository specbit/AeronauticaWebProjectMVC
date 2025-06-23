namespace FlyTickets2025.web.Repositories
{
    public interface IGenericRepository<T> where T : class // IEntity is a marker interface for entities
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id); // Changed to T? for nullability
        //Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(int id);
        Task<bool> SaveAllAsync();
    }
}
