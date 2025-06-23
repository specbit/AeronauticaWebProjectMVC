using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Username { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
