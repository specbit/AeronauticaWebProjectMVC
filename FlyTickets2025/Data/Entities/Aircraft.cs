using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.Data.Entities
{
    public class Aircraft : IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; } // e.g., "Boeing 737", "Airbus A320" 

        [Required]
        [Range(1, 500)] // Assuming reasonable seat capacity
        public int TotalSeats { get; set; } // "Consoante os tipos de aparelho disponível deverão ser criados os lugares para venda." 

        [Required]
        public string PhotoPath { get; set; } // "Os aparelhos terão de ter uma foto obrigatória" 

        // Navigation property: an aircraft can be used in many flights
        public ICollection<Flight> Flights { get; set; }
    }
}
