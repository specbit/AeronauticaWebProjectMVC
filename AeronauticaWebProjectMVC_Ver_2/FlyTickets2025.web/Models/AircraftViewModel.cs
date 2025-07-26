using FlyTickets2025.web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class AircraftViewModel : Aircraft
    {
        [Display(Name = "Image")]
        public IFormFile? AircraftImageFile { get; set; }
    }
}
