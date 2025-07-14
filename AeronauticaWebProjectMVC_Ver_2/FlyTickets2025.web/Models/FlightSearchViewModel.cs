using FlyTickets2025.web.Data.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList

namespace FlyTickets2025.web.Models
{
    public class FlightSearchViewModel
    {
        [Display(Name = "Origin City")]
        public int? OriginCityId { get; set; }

        [Display(Name = "Destination City")]
        public int? DestinationCityId { get; set; }

        [Display(Name = "Departure Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DepartureDate { get; set; }

        // This property will hold the list of flights found after a search
        public IEnumerable<Flight> AvailableFlights { get; set; } = new List<Flight>();

        // These properties are used to populate the dropdowns in the view
        // They will be populated by the controller using data from your CityRepository
        public SelectList? OriginCities { get; set; }
        public SelectList? DestinationCities { get; set; }
    }
}
