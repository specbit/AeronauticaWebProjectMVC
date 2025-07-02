using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Repositories
{
    public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
    {
        private readonly ApplicationDbContext _context;
        public AircraftRepository(ApplicationDbContext context) : base(context)
        {
            _context = context; 
        }
        // Implement specific methods for Aircraft if needed
        public async Task<bool> IsAircraftBookedOnDateAsync(int aircraftId, DateTime dateToCheck, int? currentFlightId = null)
        {
            // Query for any flights using this aircraft on the specific date
            var query = _context.Flights
                .AsNoTracking()
                .Where(f => f.AircraftId == aircraftId && f.DepartureTime.Date == dateToCheck.Date);

            // If an existing flight (being edited) is provided, exclude it from the conflict check
            if (currentFlightId.HasValue)
            {
                query = query.Where(f => f.Id != currentFlightId.Value);
            }

            return await query.AnyAsync(); // Returns true if any conflicts are found
        }

        // Implement the method to get all aircraft including associated flights
        public async Task<IEnumerable<Aircraft>> GetAllAircraftsWithFlightsAsync()
        {
            return await _context.Aircrafts
                                 .Include(a => a.Flights) // <<< Correctly includes ONLY Flights, as per your Aircraft.cs
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // Implementation to get a single aircraft including related entities for Details/Edit/Delete
        public async Task<Aircraft?> GetAircraftWithRelatedEntitiesByIdAsync(int id)
        {
            return await _context.Aircrafts
                                 .Include(a => a.Flights) // Includes associated flights
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
