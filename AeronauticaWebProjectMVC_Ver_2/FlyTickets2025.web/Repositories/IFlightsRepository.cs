using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface IFlightsRepository : IGenericRepository<Flight>
    {
        Task<bool> IsFlightNumberUniqueOnDateAsync(string flightNumber, DateTime date, int? currentFlightId = null);

        IQueryable<Flight> GetAllFlightsWithRelatedEntities();

        Task<Flight?> GetFlightWithRelatedEntitiesByIdAsync(int id);
    }
}
