using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        Task<bool> IsFlightNumberUniqueOnDateAsync(string flightNumber, DateTime date, int? currentFlightId = null);

        IQueryable<Flight> GetAllFlightsWithRelatedEntities();

        Task<Flight?> GetFlightWithRelatedEntitiesByIdAsync(int id);
        Task<IEnumerable<Flight>> GetAllFlightsAsync();
        Task<IEnumerable<Flight>> SearchFlightsAsync(int? originCityId, int? destinationCityId, DateTime? departureDate);
        Task<IEnumerable<Flight>> SearchFlightsAsync();

        Task<Flight> CreateAsync(Flight entity);
    }
}
