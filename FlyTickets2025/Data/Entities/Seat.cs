using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyTickets2025.Web.Data.Entities
{
    public enum SeatClass
    {
        [Display(Name = "Económico")]
        Economy,
        [Display(Name = "Executivo")]
        Executive
    }
    public class Seat
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

        // Foreign Key to the Aircraft (if this seat is part of an Aircraft's template layout, not tied to a specific flight instance)
        // It's nullable because a specific seat on a *flight* might not directly link back to an Aircraft *template*.
        // If 'Seat' is ONLY ever created per flight, this AircraftId might not be necessary.
        [Display(Name = "Modelo Aparelho")]
        public required int? AircraftId { get; set; } // Nullable, as a flight seat might just derive from a template, not directly reference it.
        [ForeignKey("AircraftId")]
        public Aircraft? AircraftModel { get; set; } // Nullable navigation property to Aircraft template

        // Foreign Key to the Ticket that purchased this seat (nullable, as a seat might not be purchased yet)
        // This implies a 1-to-1 or 1-to-0..1 relationship from Ticket to Seat.
        // When a ticket is purchased, this ID will be set.
        [Display(Name = "Bilhete Associado")]
        public int? TicketId { get; set; } // Nullable as a seat can exist without a ticket
        [ForeignKey("TicketId")]
        public Ticket? Ticket { get; set; } // Nullable navigation property
        
    }
}
