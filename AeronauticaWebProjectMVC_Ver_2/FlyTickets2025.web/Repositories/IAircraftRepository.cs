using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface IAircraftRepository : IGenericRepository<Aircraft>
    {
        Task<bool> IsAircraftBookedOnDateAsync(int aircraftId, DateTime dateToCheck, int? currentFlightId = null);
        Task<IEnumerable<Aircraft>> GetAllAircraftsWithFlightsAsync();
        Task<Aircraft?> GetAircraftWithRelatedEntitiesByIdAsync(int id);
        Task<bool> HasSoldTicketsAsync(int aircraftId);
        Task<bool> HasAssociatedFlightsAsync(int aircraftId);
    }
}
