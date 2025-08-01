using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Ticket> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task AddAsync(Ticket ticket)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TicketExistsAsync(int id)
        {
            return await _context.Tickets.AnyAsync(e => e.Id == id);
        }

        // Implementation of methods with Includes
        public async Task<IEnumerable<Ticket>> GetAllWithUsersFlightsAndSeatsAsync()
        {
            return await _context.Tickets
                .Include(t => t.ApplicationUser)
                .Include(t => t.Flight)
                .Include(t => t.Seat)
                .ToListAsync();
        }

        public async Task<Ticket> GetByIdWithUsersFlightsAndSeatsAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.ApplicationUser)
                .Include(t => t.Flight)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> HasTicketsForUserAsync(string userId)
        {
            return await _context.Tickets.AnyAsync(t => t.ApplicationUserId == userId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId)
        {
            // The query will get all tickets and then filter them in the database.
            return await _context.Tickets
                // Use a 'Where' clause to filter the tickets by the user's ID.
                // Assuming your Ticket entity has a foreign key property named ApplicationUserId.
                .Where(t => t.ApplicationUserId == userId)

                // Include related entities to avoid null reference exceptions in your view.
                // The 'Include' statement ensures that Flight and Seat data is loaded with each ticket.
                .Include(t => t.Flight)
                .Include(t => t.Seat)

                // You can add an OrderBy clause to sort the tickets, for example, by purchase date.
                .OrderByDescending(t => t.PurchaseDate)

                // Execute the query and return the list of tickets.
                .ToListAsync();
        }
    }
}
