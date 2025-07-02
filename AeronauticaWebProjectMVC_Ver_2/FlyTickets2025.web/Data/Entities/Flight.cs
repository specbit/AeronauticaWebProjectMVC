using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FlyTickets2025.Web.ValidationAttributes;
using FlyTickets2025.web.ValidationAttributes;

namespace FlyTickets2025.web.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Número de vôo")]
        [Required]
        [StringLength(20)]
        [UniqueFlightOnDate(ErrorMessage = "A flight with this number already exists on the selected date.")]
        public string? FlightNumber { get; set; } // e.g., "TP123", "BA456" 

        [Display(Name = "Data e hora de partida")]
        [Required]
        [DataType(DataType.DateTime)]
        [TodayDateMinimun]
        public DateTime DepartureTime { get; set; } // "data, hora" 

        [Display(Name = "Duração (minutos)")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A duração do voo deve ser maior que zero e um número inteiro.")]
        public required int DurationMinutes { get; set; }

        // Foreign Key for Origin City
        [Display(Name = "Origem")]
        [Required]
        public int OriginCityId { get; set; }
        [ForeignKey("OriginCityId")]
        public City? OriginCity { get; set; } // "origem" 

        // Foreign Key for Destination City
        [Display(Name = "Destino")]
        [Required]
        [NotSameOriginDestination(ErrorMessage = "A cidade de Destino não pode ser a mesma que a Origem.")]
        //[Compare("OriginCityId", ErrorMessage = "A cidade de Destino não pode ser a mesma que a Origem.")]
        public int DestinationCityId { get; set; }
        [ForeignKey("DestinationCityId")]
        public City? DestinationCity { get; set; } // "destino" 

        // Foreign Key for Aircraft
        [Display(Name = "Aparelho")]
        [Required]
        [NotBookedOnDate(ErrorMessage = "This aircraft is already booked for another flight on this date.")]
        public int AircraftId { get; set; }
        [ForeignKey("AircraftId")]
        public Aircraft? Aircraft { get; set; } // "aparelho" 

        // Navigation property: a flight can have many tickets
        public ICollection<Ticket>? Tickets { get; set; }

        // Navigation property: a flight has many Seat instances 
        public ICollection<Seat>? Seats { get; set; }
    }
}
