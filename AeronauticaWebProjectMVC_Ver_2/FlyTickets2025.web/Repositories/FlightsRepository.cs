// Repositories/FlightsRepository.cs
using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Repositories;
using Microsoft.EntityFrameworkCore; // Para .Include(), .AsNoTracking(), .AnyAsync(), .FirstOrDefaultAsync(), .ToListAsync()

namespace FlyTickets2025.Web.Repositories
{
    public class FlightsRepository : GenericRepository<Flight>, IFlightsRepository
    {
        private readonly ApplicationDbContext _context; // Referência ao DbContext para queries específicas (como Includes)

        public FlightsRepository(ApplicationDbContext context) : base(context) // Passa o contexto para o construtor da classe base (GenericRepository)
        {
            _context = context; // Armazena o contexto para uso em métodos específicos
        }

        // Implementação do método para verificar se o número do voo é único numa data
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

        // Implementação para obter um único voo com as suas entidades relacionadas por ID
        public async Task<Flight?> GetFlightWithRelatedEntitiesByIdAsync(int id)
        {
            return await _context.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.DestinationCity)
                .Include(f => f.OriginCity)
                .AsNoTracking() // Não rastreia a entidade
                .FirstOrDefaultAsync(f => f.Id == id); // Encontra o primeiro que corresponde ao ID
        }

        // Métodos como CreateAsync, UpdateAsync, DeleteAsync (por ID), GetByIdAsync, GetAllAsync, ExistsAsync
        // são herdados automaticamente do GenericRepository<Flight> e já incluem SaveAllAsync() internamente.
    }
}