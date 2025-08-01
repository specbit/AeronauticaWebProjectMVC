using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Repositories;
using Microsoft.EntityFrameworkCore; // (PT)Para .Include(), .AsNoTracking(), .AnyAsync(), .FirstOrDefaultAsync(), .ToListAsync()

namespace FlyTickets2025.Web.Repositories
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly ApplicationDbContext _context; // (PT)Referência ao DbContext para queries específicas (como Includes)

        public FlightRepository(ApplicationDbContext context) : base(context) // (PT)Passa o contexto para o construtor da classe base (GenericRepository)
        {
            _context = context; // (PT)Armazena o contexto para uso em métodos específicos
        }

        // (PT)Implementação do método para verificar se o número do voo é único numa data
        // Retorna true se houver conflito (não único), false se for único.
        public async Task<bool> IsFlightNumberUniqueOnDateAsync(string flightNumber, DateTime date, int? currentFlightId = null)
        {
            var query = _context.Flights
                .AsNoTracking() // Não rastreia as entidades para esta query de leitura
                .Where(f => f.FlightNumber == flightNumber &&
                            f.DepartureTime.Date == date.Date); // Compara apenas a data

            // Exclui o voo atual se estivermos numa operação de edição
            if (currentFlightId.HasValue)
            {
                query = query.Where(f => f.Id != currentFlightId.Value);
            }

            return await query.AnyAsync(); // Retorna true se encontrar algum voo que cause conflito
        }

        // Implementação para obter todos os voos com as suas entidades relacionadas
        // (Aeronave, Cidade de Origem, Cidade de Destino)
        public IQueryable<Flight> GetAllFlightsWithRelatedEntities()
        {
            return _context.Flights
                .Include(f => f.Aircraft) // Inclui a aeronave
                .Include(f => f.DestinationCity) // Inclui a cidade de destino
                .Include(f => f.OriginCity) // Inclui a cidade de origem
                .AsNoTracking(); // Retorna um IQueryable não rastreado para listagens (leitura)
        }

        // Implementation for getting a single flight with its related entities by ID
        public async Task<Flight?> GetFlightWithRelatedEntitiesByIdAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.DestinationCity)
                .Include(f => f.OriginCity)
                .Include(f => f.Tickets) // Includes tickets if necessary
                .Include(f => f.Seats) // Includes seats if necessary
                .AsNoTracking() // No tracking for Entity Framework Core
                .FirstOrDefaultAsync(f => f.Id == id); // Finds the first that matches the ID
        }
        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            return await _context.Flights.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync(int? originCityId, int? destinationCityId, DateTime? departureDate)
        {
            // Start with a query that includes necessary related entities
            var query = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity).Where(x => x.DepartureTime >= DateTime.Now)
                .AsNoTracking(); // Good for read-only search results

            // Apply filters based on provided parameters
            if (originCityId.HasValue && originCityId.Value > 0)
            {
                query = query.Where(f => f.OriginCity.Id == originCityId.Value);
            }

            if (destinationCityId.HasValue && destinationCityId.Value > 0)
            {
                query = query.Where(f => f.DestinationCity.Id == destinationCityId.Value);
            }

            if (departureDate.HasValue)
            {
                // Compare only the date part to ignore time differences
                query = query.Where(f => f.DepartureTime.Date == departureDate.Value.Date);
            }

            // Order the results, for example, by departure time
            query = query.OrderBy(f => f.DepartureTime);

            // Execute the query and return the results
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Flight>> SearchFlightsAsync() // No parameters, return all flights with related entities
        {
            // Start with a query that includes necessary related entities
            var query = _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.OriginCity)
                .Include(f => f.DestinationCity).Where(x => x.DepartureTime >= DateTime.Now)
                .AsNoTracking(); // Good for read-only search results

            // Order the results, for example, by departure time
            query = query.OrderBy(f => f.DepartureTime);

            // Execute the query and return the results
            return await query.ToListAsync();
        }

        public new async Task<Flight> CreateAsync(Flight entity)
        {
            entity.SetEstimateArrival();
            await _context.Flights.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity; // Return the created flight
        }

        //public async Task<bool> IsAircraftBookedForPeriodAsync(int aircraftId, DateTime newDeparture, DateTime newArrival)
        //{
        //    // Checks if any other flight for the same aircraft has an overlapping time period.
        //    return await _context.Flights
        //        .AnyAsync(f =>
        //            f.AircraftId == aircraftId &&
        //            f.DepartureTime < newArrival &&
        //            f.EstimateArrival > newDeparture
        //        );
        //}
        public async Task<bool> IsAircraftBookedForPeriodAsync(int aircraftId, DateTime newDeparture, DateTime newArrival)
        {
            // 1. Fetch all flights for this specific aircraft into memory.
            var existingFlightsForAircraft = await _context.Flights
                .Where(f => f.AircraftId == aircraftId)
                .AsNoTracking()
                .ToListAsync();

            // 2. Now, perform the overlap check in C# memory, where the calculated
            //    arrival time can be used without causing an error.
            return existingFlightsForAircraft.Any(f =>
                f.DepartureTime < newArrival &&
                f.DepartureTime.AddMinutes(f.DurationMinutes) > newDeparture
            );
        }

        //public async Task<IEnumerable<Aircraft>> GetAvailableAircraftForFlightAsync(int flightId)
        //{
        //    var currentFlight = await _context.Flights
        //        .Include(f => f.Tickets)
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(f => f.Id == flightId);

        //    if (currentFlight == null) return new List<Aircraft>();

        //    int ticketsSold = currentFlight.Tickets.Count;
        //    var currentFlightArrival = currentFlight.DepartureTime.AddMinutes(currentFlight.DurationMinutes);

        //    // 1. Fetch all other flights from the database into memory
        //    var allOtherFlights = await _context.Flights
        //        .Where(f => f.Id != flightId)
        //        .AsNoTracking()
        //        .ToListAsync();

        //    // 2. Now, filter the list in C# to find busy aircraft
        //    var busyAircraftIds = allOtherFlights
        //        .Where(f =>
        //            f.DepartureTime.AddMinutes(f.DurationMinutes).AddHours(24) > currentFlight.DepartureTime &&
        //            f.DepartureTime < currentFlightArrival.AddHours(24))
        //        .Select(f => f.AircraftId)
        //        .Distinct()
        //        .ToList();

        //    // 3. Return only the aircraft that have enough seats AND are not busy
        //    return await _context.Aircrafts
        //        .AsNoTracking()
        //        .Where(a => a.TotalSeats >= ticketsSold && !busyAircraftIds.Contains(a.Id))
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<Aircraft>> GetAvailableAircraftForFlightAsync(int flightId)
        {
            var currentFlight = await _context.Flights
                .Include(f => f.Tickets)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (currentFlight == null) return new List<Aircraft>();

            int ticketsSold = currentFlight.Tickets.Count;
            var currentFlightArrival = currentFlight.DepartureTime.AddMinutes(currentFlight.DurationMinutes);

            // 1. Fetch all other flights from the database into memory.
            var allOtherFlights = await _context.Flights
                .Where(f => f.Id != flightId)
                .AsNoTracking()
                .ToListAsync();

            // 2. Filter the list in C# to find which aircraft are busy.
            var busyAircraftIds = allOtherFlights
                .Where(f =>
                {
                    var otherFlightArrival = f.DepartureTime.AddMinutes(f.DurationMinutes);

                    // Check if the other flight starts less than 24 hours after our current flight ends.
                    bool startsTooSoon = f.DepartureTime > currentFlightArrival &&
                                         f.DepartureTime < currentFlightArrival.AddHours(24);

                    // Check if the other flight ends less than 24 hours before our current flight starts.
                    bool endsTooLate = currentFlight.DepartureTime > otherFlightArrival &&
                                       currentFlight.DepartureTime < otherFlightArrival.AddHours(24);

                    return startsTooSoon || endsTooLate;
                })
                .Select(f => f.AircraftId)
                .Distinct()
                .ToList();

            // 3. Return only the aircraft that have enough seats AND are not in the busy list.
            return await _context.Aircrafts
                .AsNoTracking()
                .Where(a => a.TotalSeats >= ticketsSold && !busyAircraftIds.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<bool> HasAssociatedTicketsOrSeatsAsync(int flightId)
        {
            // Assuming your Ticket entity has a direct FlightId foreign key.
            return await _context.Tickets.AnyAsync(t => t.FlightId == flightId);

            // If Ticket only links to SeatId, and Seat links to FlightId,
            // you'd use a more complex check like:
            // return await _context.Seats
            //     .Where(s => s.FlightId == flightId)
            //     .AnyAsync(s => s.Tickets.Any()); // Requires Seat to have ICollection<Ticket> Tickets
        }
    }
}