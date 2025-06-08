using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace FlyTickets2025.Web.Data.Entities
{
    public class Flight
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string FlightNumber { get; set; } // e.g., "TP123", "BA456" 

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; } // "data, hora" 

        // It's good practice to have an arrival time for a flight
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; }

        // Foreign Key for Origin City
        public int OriginCityId { get; set; }
        [ForeignKey("OriginCityId")]
        public City OriginCity { get; set; } // "origem" 

        // Foreign Key for Destination City
        public int DestinationCityId { get; set; }
        [ForeignKey("DestinationCityId")]
        public City DestinationCity { get; set; } // "destino" 

        // Foreign Key for Aircraft
        public int AircraftId { get; set; }
        [ForeignKey("AircraftId")]
        public Aircraft Aircraft { get; set; } // "aparelho" 

        // Navigation property: a flight can have many tickets
        public ICollection<Ticket> Tickets { get; set; }
    }
}
