using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string? ProfilePicturePath { get; set; } // For optional user profile photos

        // Navigation property for Client's tickets/bookings
        public ICollection<Ticket> Tickets { get; set; }
    }
}
