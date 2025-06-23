using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Data;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Repositories
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
                                 .AsNoTracking() // Good for read-only
                                 .ToListAsync();
        }
    }
}
