using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Data.Entities
{
    public enum PassengerType
    {
        [Display(Name = "Adulto")]
        Adult,
        [Display(Name = "Criança")]
        Child,
        [Display(Name = "Bebé")]
        Infant
    }
    public class Ticket : IEntity
    {
        public Ticket()
        {
            
        }

        public int Id { get; set; }

        // Foreign Key to the Flight
        [Display(Name = "Voo")]
        [Required] // A ticket must be associated with a flight
        public int FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }// Links to the flight for which the ticket is bought 

        // Foreign Key to the ApplicationUser (Client)
        [Display(Name = "Utilizador Associado")] // Display name added
        [Required]
        public string ApplicationUserId { get; set; } // IdentityUser's Id is a string (GUID)
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } // Links to the client who bought the ticket 

        // Foreign Key to the specific Seat this ticket is for
        [Display(Name = "Assento Selecionado")] // "escolher o lugar pretendido"
        [Required] // A ticket must have a seat
        public int SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; } // Navigation to the Seat object 

        // Property to store PassengerType
        [Display(Name = "Tipo de Passageiro")]
        [Required]
        public PassengerType PassengerType { get; set; } // Uses the enum

        // Use a simple bool, or derive availability from the existence of a ticket for that seat
        // "devendo depois de escolhido, ficar indisponível para os próximos compradores." 
        public bool IsBooked { get; set; } // Could be set to true upon purchase.

        [DataType(DataType.DateTime)]
        [Display(Name = "Data da Compra")] // Display name added
        [Required]
        public DateTime PurchaseDate { get; set; } // When the ticket was bought

        [Display(Name = "Preço do Bilhete")]
        [Required]
        public decimal TicketPrice { get; set; }

        [Display(Name = "Nome do Cliente")]
        [Required]
        public string ClientName { get; set; }

        [Display(Name = "Data do Voo")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime FlightDate { get; set; }
    }
}
