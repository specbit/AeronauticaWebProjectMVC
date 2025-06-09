using FlyTickets2025.Data;
using FlyTickets2025.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.Web.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Implement the specific method(s) from ICityRepository
        public async Task<IEnumerable<City>> GetAllCitiesWithFlightsAsync()
        {
            // Example of including related entities
            return await _context.Cities
                                 .Include(c => c.OriginFlights) // Eagerly load originating flights
                                 .Include(c => c.DestinationFlights) // Eagerly load destination flights
                                 .AsNoTracking() // Good for read-only
                                 .ToListAsync();
        }
    }
}
