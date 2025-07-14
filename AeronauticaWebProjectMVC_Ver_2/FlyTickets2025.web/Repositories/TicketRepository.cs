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
    }
}
