using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!; // e.g., "Lisboa", "New York" 

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = null!; // e.g., "Portugal", "USA" 

        // Property for the flag image path
        // "deverá aparecer a bandeira referente aos países das cidades de origem e destino." 
        public string FlagImagePath { get; set; } = null!; // e.g., "/images/flags/portugal.png"

        // Navigation properties 
        // A city can be an origin or destination for many flights.
        public ICollection<Flight>? OriginFlights { get; set; }
        public ICollection<Flight>? DestinationFlights { get; set; }
    }
}
