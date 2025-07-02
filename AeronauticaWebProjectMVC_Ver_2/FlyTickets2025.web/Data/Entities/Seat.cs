using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Data.Entities
{
    // TODO: Move to a separate file 
    public enum SeatClass
    {
        [Display(Name = "Económico")]
        Economy,
        [Display(Name = "Executivo")]
        Executive
    }

    public class Seat : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Número do Assento")]
        [Required]
        [StringLength(10)]
        public required string SeatNumber { get; set; } // e.g., "A1", "12B"

        [Display(Name = "Tipo de Assento")]
        [Required]
        public required SeatClass Type { get; set; } // Uses the new enum

        [Display(Name = "Preço")]
        [Required]
        [Column(TypeName = "decimal(18, 2)")] // Ensure proper decimal storage for currency
        public required decimal Value { get; set; } // Use decimal for currency

        // Indicates if the seat is currently occupied (booked)
        [Display(Name = "Disponível")]
        public bool IsAvailableForSale { get; set; } = true;

        // Foreign Key to the Flight this seat belongs to
        // A Seat exists for a specific Flight instance
        [Display(Name = "Voo")]
        [Required]
        public required int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight? Flight { get; set; } // Nullable navigation property

        //TODO: Review if this is necessary, as a Seat might not always be tied to an Aircraft template.

        // Foreign Key to the Aircraft 
        [Display(Name = "Modelo Aparelho")]
        public required int AircraftId { get; set; } // Nullable, as a flight seat might just derive from a template, not directly reference it.
        [ForeignKey("AircraftId")]
        public Aircraft? AircraftModel { get; set; } // Nullable navigation property to Aircraft template
    }
}
