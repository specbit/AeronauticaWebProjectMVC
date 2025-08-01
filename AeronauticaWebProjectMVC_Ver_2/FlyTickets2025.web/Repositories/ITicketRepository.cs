using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<Ticket> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
        Task<bool> TicketExistsAsync(int id);
        Task<IEnumerable<Ticket>> GetAllWithUsersFlightsAndSeatsAsync();
        Task<Ticket> GetByIdWithUsersFlightsAndSeatsAsync(int id);
        Task<bool> HasTicketsForUserAsync(string userId);
        Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId);
    }
}