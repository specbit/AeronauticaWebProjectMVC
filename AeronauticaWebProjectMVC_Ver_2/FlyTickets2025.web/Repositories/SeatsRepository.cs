using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Repositories
{
    public class SeatsRepository : GenericRepository<Seat>, ISeatsRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Implementation of specific methods for Seats
        public async Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId)
        {
            return await _context.Seats
                                 .Where(s => s.FlightId == flightId && s.IsAvailableForSale == true)
                                 .Include(s => s.Flight) // Include the related Flight
                                 .Include(s => s.AircraftModel) // Include the related AircraftModel
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetSeatsByAircraftModelIdAsync(int aircraftModelId)
        {
            return await _context.Seats
                                 .Where(s => s.AircraftId == aircraftModelId)
                                 .Include(s => s.Flight) // Include the related Flight
                                 .Include(s => s.AircraftModel) // Include the related AircraftModel
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Seat?> GetSeatWithRelatedEntitiesByIdAsync(int id)
        {
            return await _context.Seats
                                 .Include(s => s.Flight)
                                 .Include(s => s.AircraftModel)
                                 //.Include(s => s.Ticket) // Include Ticket if needed for details
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Implementation of the method to get all seats including related entities for Index view
        public async Task<IEnumerable<Seat>> GetAllSeatsWithRelatedEntitiesAsync()
        {
            return await _context.Seats
                                 .Include(s => s.Flight) // Include the related Flight
                                 .Include(s => s.AircraftModel) // Include the related AircraftModel
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetAllSeatsAsync()
        {
            return await _context.Seats.AsNoTracking().ToListAsync();
        }
    }
}
