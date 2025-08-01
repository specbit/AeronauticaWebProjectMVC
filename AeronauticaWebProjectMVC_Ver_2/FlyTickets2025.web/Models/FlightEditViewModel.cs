using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class FlightEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de voo")]
        public string FlightNumber { get; set; }

        [Display(Name = "Partida")]
        public DateTime DepartureTime { get; set; }

        [Display(Name = "Duração (minutos)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Origin City")]
        public int OriginCityId { get; set; }
        public SelectList? OriginCityList { get; set; }

        [Display(Name = "Destination City")]
        public int DestinationCityId { get; set; }
        public SelectList? DestinationCityList { get; set; }

        [Display(Name = "Aircraft")]
        public int AircraftId { get; set; }
        public SelectList? AircraftList { get; set; }
    }
}
