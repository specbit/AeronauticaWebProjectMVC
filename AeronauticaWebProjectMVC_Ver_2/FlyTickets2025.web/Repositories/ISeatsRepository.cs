using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface ISeatsRepository : IGenericRepository<Seat>
    {
        // Add a method to get all seats including related entities for Index view
        Task<IEnumerable<Seat>> GetAllSeatsWithRelatedEntitiesAsync();

        // To get seats for a specific flight:
        Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId);

        // To get seats for a specific aircraft model:
        Task<IEnumerable<Seat>> GetSeatsByAircraftModelIdAsync(int aircraftModelId);

        // To get a specific seat with its related Flight and AircraftModel for details:
        Task<Seat?> GetSeatWithRelatedEntitiesByIdAsync(int id);
        Task<IEnumerable<Seat>> GetAllSeatsAsync();
    }
}
