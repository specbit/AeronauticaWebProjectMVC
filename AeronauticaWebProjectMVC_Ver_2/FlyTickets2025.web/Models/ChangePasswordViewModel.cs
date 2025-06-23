using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current Password")]
        public required string OldPassword { get; set; }
        [Required]
        [Display(Name = "New Password")]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public required string Confirm { get; set; }
    }
}
