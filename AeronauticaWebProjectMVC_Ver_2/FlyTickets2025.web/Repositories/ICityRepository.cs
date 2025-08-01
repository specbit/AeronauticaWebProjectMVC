using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<IEnumerable<City>> GetAllCitiesWithFlightsAsync();
        Task<IEnumerable<City>> GetAllCitiesAsync();
        Task<bool> HasAssociatedFlightsAsync(int cityId);
    }
}
