using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyTickets2025.Web.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Número de vôo")]
        [Required]
        [StringLength(20)]
        public string FlightNumber { get; set; } = null!;// e.g., "TP123", "BA456" 

        [Display(Name = "Data e hora de partida")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; } // "data, hora" 

        [Display(Name = "Duração (minutos)")] 
        [Required]
        //[Range(1, 1440)] // Example range, adjust as needed
        public required int DurationMinutes { get; set; }

        // Foreign Key for Origin City
        [Display(Name = "Origem")]
        [Required]
        public int OriginCityId { get; set; }
        [ForeignKey("OriginCityId")]
        public City OriginCity { get; set; } = null!;// "origem" 

        // Foreign Key for Destination City
        [Display(Name = "Destino")]
        [Required]
        public int DestinationCityId { get; set; }
        [ForeignKey("DestinationCityId")]
        public City DestinationCity { get; set; } = null!;// "destino" 

        // Foreign Key for Aircraft
        [Display(Name = "Aparelho")]
        [Required]
        public int AircraftId { get; set; }
        [ForeignKey("AircraftId")]
        public Aircraft Aircraft { get; set; } = null!;// "aparelho" 

        // Navigation property: a flight can have many tickets
        public ICollection<Ticket>? Tickets { get; set; }

        // Navigation property: a flight has many Seat instances 
        public ICollection<Seat>? Seats { get; set; }
    }
}
