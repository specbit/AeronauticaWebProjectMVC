﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FlyTickets2025.Web.ValidationAttributes;
using FlyTickets2025.web.ValidationAttributes;

namespace FlyTickets2025.web.Data.Entities
{
    public class Flight : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Número de vôo")]
        [Required]
        [StringLength(20)]
        [UniqueFlightOnDate(ErrorMessage = "A flight with this number already exists on the selected date.")]
        public string? FlightNumber { get; set; } // e.g., "TP123", "BA456" 

        [Display(Name = "Partida")]
        [Required]
        [DataType(DataType.DateTime)]
        [TodayDateMinimun]
        public DateTime DepartureTime { get; set; } // "data, hora" 

        [Display(Name = "Duração (minutos)")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A duração do voo deve ser maior que zero e um número inteiro.")]
        public required int DurationMinutes { get; set; }

        // Foreign Key for Origin City
        [Required]
        public int OriginCityId { get; set; }
        [ForeignKey("OriginCityId")]
        [Display(Name = "Origem")]
        public City? OriginCity { get; set; } // "origem" 

        // Foreign Key for Destination City
        [Required]
        [NotSameOriginDestination(ErrorMessage = "A cidade de Destino não pode ser a mesma que a Origem.")]
        //[Compare("OriginCityId", ErrorMessage = "A cidade de Destino não pode ser a mesma que a Origem.")]
        public int DestinationCityId { get; set; }
        [ForeignKey("DestinationCityId")]
        [Display(Name = "Destino")]
        public City? DestinationCity { get; set; } // "destino" 

        // Foreign Key for Aircraft
        [Required]
        [NotBookedOnDate(ErrorMessage = "This aircraft is already booked for another flight on this date.")]
        public int AircraftId { get; set; }
        [ForeignKey("AircraftId")]
        [Display(Name = "Aparelho")]
        public Aircraft? Aircraft { get; set; } // "aparelho" 

        [NotMapped]
        [Display(Name = "Chegada")]
        public DateTime EstimateArrival { get; set; }

        [NotMapped]
        //[Display(Name = "Default Flight Value")]
        //[Range(1, double.MaxValue, ErrorMessage = "O valor defeito do voo deve ser maior que zero.")]
        public decimal DefaultFlightValue { get; set; }

        [NotMapped]
        //[Display(Name = "Number Of Executive Seats")]
        //[Range(1, int.MaxValue, ErrorMessage = "O número por defeito de percentagem de  voo deve ser maior que zero e um número inteiro.")]
        public int PercentageOfExecutiveSeats { get; set; }

        // Navigation property: a flight can have many tickets
        public ICollection<Ticket>? Tickets { get; set; }

        // Navigation property: a flight has many Seat instances 
        public ICollection<Seat>? Seats { get; set; }

        public void SetEstimateArrival()
        {
            // Calculate the estimated arrival time based on departure time and duration
            EstimateArrival = DepartureTime.AddMinutes(DurationMinutes);
        }
    }
}
