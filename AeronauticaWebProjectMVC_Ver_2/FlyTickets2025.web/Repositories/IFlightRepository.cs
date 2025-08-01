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
        //new Task<Flight> CreateAsync(Flight entity);
        Task<bool> IsAircraftBookedForPeriodAsync(int aircraftId, DateTime newDeparture, DateTime newArrival);
        Task<IEnumerable<Aircraft>> GetAvailableAircraftForFlightAsync(int flightId);
        Task<bool> HasAssociatedTicketsOrSeatsAsync(int flightId);
    }
}
