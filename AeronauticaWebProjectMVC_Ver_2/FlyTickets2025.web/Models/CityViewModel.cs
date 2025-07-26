using FlyTickets2025.web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class CityViewModel : City // CityViewModel inherits from City to access entity City properties
    {
        [Display(Name = "Image")]
        public IFormFile? FlagImageFile { get; set; } // Property to hold the uploaded flag image file
    }
}
