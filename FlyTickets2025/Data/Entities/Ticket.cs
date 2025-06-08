using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.Data.Entities
{
    public class Ticket
    {
        public int Id { get; set; }

        // Foreign Key to the Flight
        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight Flight { get; set; } // Links to the flight for which the ticket is bought 

        // Foreign Key to the ApplicationUser (Client)
        public string ApplicationUserId { get; set; } // IdentityUser's Id is a string (GUID)
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } // Links to the client who bought the ticket 

        [Required]
        [StringLength(10)]
        public string SeatNumber { get; set; } // "escolher o lugar pretendido" 

        // You could use a simple bool, or derive availability from the existence of a ticket for that seat
        // "devendo depois de escolhido, ficar indisponível para os próximos compradores." 
        public bool IsBooked { get; set; } // Could be set to true upon purchase.

        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; } // When the ticket was bought
    }
}
