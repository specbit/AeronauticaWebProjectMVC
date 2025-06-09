using FlyTickets2025.Web.Data.Entities;

namespace FlyTickets2025.Web.Repositories
{
    public interface ICityRepository : IGenericRepository<City>
    {
        Task<IEnumerable<City>> GetAllCitiesWithFlightsAsync();
    }
}
