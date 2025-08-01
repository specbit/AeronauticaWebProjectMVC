using FlyTickets2025.web.Data.Entities;

namespace FlyTickets2025.web.Repositories
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        // Add a method to get all seats including related entities for Index view
        Task<IEnumerable<Seat>> GetAllSeatsWithRelatedEntitiesAsync();

        // To get seats for a specific flight:
        Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId);

        // To get seats for a specific aircraft model:
        Task<IEnumerable<Seat>> GetSeatsByAircraftModelIdAsync(int aircraftModelId);

        // To get a specific seat with its related Flight and AircraftModel for details:
        Task<Seat?> GetSeatWithRelatedEntitiesByIdAsync(int id);

        /// <summary>
        /// Retrieves all seats without any related entities.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Seat>> GetAllSeatsAsync();

        /// <summary>
        /// Creates seats for a flight based on the flight's aircraft model.
        /// </summary>
        /// <param name="flightId"></param>
        /// <param name="defaultValue"></param>
        /// <param name="percentageOfExecutiveSeats"></param>
        /// <returns></returns>
        Task<IEnumerable<Seat>> CreateSeatsForFlightAsync(int flightId, decimal defaultValue, int percentageOfExecutiveSeats);

        /// <summary>
        /// Checks if a seat has any associated tickets.
        /// </summary>
        /// <param name="seatId"></param>
        /// <returns>True if a seat has a Ticket</returns>
        Task<bool> HasAssociatedTicketsAsync(int seatId);
    }
}
