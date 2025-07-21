using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Repositories
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlightRepository _flightRepository;

        public SeatRepository(ApplicationDbContext context, IFlightRepository flightRepository) : base(context)
        {
            _context = context;
            _flightRepository = flightRepository;
        }

        // Implementation of specific methods for Seats
        public async Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId)
        {
            return await _context.Seats
                                 .Where(s => s.FlightId == flightId && s.IsAvailableForSale == true)
                                 .Include(s => s.Flight) // Include the related Flight
                                 .Include(s => s.AircraftModel) // Include the related AircraftModel
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetSeatsByAircraftModelIdAsync(int aircraftModelId)
        {
            return await _context.Seats
                                 .Where(s => s.AircraftId == aircraftModelId)
                                 .Include(s => s.Flight) // Include the related Flight
                                 .Include(s => s.AircraftModel) // Include the related AircraftModel
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Seat?> GetSeatWithRelatedEntitiesByIdAsync(int id)
        {
            return await _context.Seats
                                 .Include(s => s.Flight)
                                 .Include(s => s.AircraftModel)
                                 //.Include(s => s.Ticket) // Include Ticket if needed for details
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Implementation of the method to get all seats including related entities for Index view
        public async Task<IEnumerable<Seat>> GetAllSeatsWithRelatedEntitiesAsync()
        {
            return await _context.Seats
                                 .Include(s => s.Flight) // Include the related Flight
                                 .Include(s => s.AircraftModel) // Include the related AircraftModel
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Seat>> GetAllSeatsAsync()
        {
            return await _context.Seats.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Seat>> CreateSeatsForFlightAsync(int flightId, decimal defaultValue, int percentageOfExecutiveSeats)
        {
            var flight = await _flightRepository.GetFlightWithRelatedEntitiesByIdAsync(flightId);

            if (flight == null || flight.Aircraft == null)
                throw new Exception("Voo ou aeronave não encontrados.");

            int totalSeats = flight.Aircraft.TotalSeats;
            int executiveSeatsCount = (int)Math.Floor(totalSeats * (percentageOfExecutiveSeats / 100m));
            int executiveCreated = 0;

            var seats = new List<Seat>();

            int number = 1;    // número sequencial para os assentos
            while (seats.Count < totalSeats)
            {
                // Para cada número criamos 4 assentos na sequência: A{n}, A{n+1}, B{n}, B{n+1}
                // Aqui vamos criar dois "pares" por iteração (4 assentos)

                string[] seatNumbers = {
            $"A{number}",
            $"A{number + 1}",
            $"B{number}",
            $"B{number + 1}"
        };

                foreach (var seatNumber in seatNumbers)
                {
                    if (seats.Count >= totalSeats)
                        break;

                    SeatClass seatClass = executiveCreated < executiveSeatsCount ? SeatClass.Executive : SeatClass.Economy;
                    decimal seatValue = seatClass == SeatClass.Executive
                        ? Math.Round(defaultValue * 1.3m, 2)
                        : defaultValue;

                    seats.Add(new Seat
                    {
                        FlightId = flightId,
                        AircraftId = flight.Aircraft.Id,
                        SeatNumber = seatNumber,
                        Type = seatClass,
                        Value = seatValue,
                        IsAvailableForSale = true
                    });

                    if (seatClass == SeatClass.Executive)
                        executiveCreated++;
                }

                number += 2; // incrementa de 2 em 2 para seguir o padrão (A1, A2, A3, A4...)
            }

            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return seats;
        }
    }
}
