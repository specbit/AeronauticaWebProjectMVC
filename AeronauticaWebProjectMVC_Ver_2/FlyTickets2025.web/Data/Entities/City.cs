using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; } // e.g., "Lisboa", "New York" 

        [Required]
        [Display(Name = "Airport Name")]
        public string? AirportName { get; set; }

        [Required]
        [StringLength(100)]
        public string? Country { get; set; } // e.g., "Portugal", "USA" 

        // Property for the flag image path
        // "deverá aparecer a bandeira referente aos países das cidades de origem e destino." 
        public string? FlagImagePath { get; set; }  // e.g., "/images/flags/portugal.png"
    }
}
