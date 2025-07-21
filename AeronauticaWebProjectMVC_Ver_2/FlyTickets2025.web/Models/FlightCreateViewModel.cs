using FlyTickets2025.web.ValidationAttributes;
using FlyTickets2025.Web.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class FlightCreateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de vôo")]
        [Required]
        [StringLength(20)]
        [UniqueFlightOnDate(ErrorMessage = "A flight with this number already exists on the selected date.")]
        public string? FlightNumber { get; set; }

        [Display(Name = "Partida")]
        [Required]
        [DataType(DataType.DateTime)]
        [TodayDateMinimun]
        public DateTime DepartureTime { get; set; }

        [Display(Name = "Duração (minutos)")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A duração do voo deve ser maior que zero e um número inteiro.")]
        public int DurationMinutes { get; set; }

        [Required]
        public int OriginCityId { get; set; }

        [Required]
        [NotSameOriginDestination(ErrorMessage = "A cidade de Destino não pode ser a mesma que a Origem.")]
        public int DestinationCityId { get; set; }

        [Required]
        [NotBookedOnDate(ErrorMessage = "This aircraft is already booked for another flight on this date.")]
        public int AircraftId { get; set; }

        // Campos que estavam a dar problemas, agora sem [NotMapped]
        [Display(Name = "Valor Padrão do Voo")]
        [Range(1, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal DefaultFlightValue { get; set; }

        [Display(Name = "Percentagem de Assentos Executivos")]
        [Range(1, 100, ErrorMessage = "A percentagem deve ser entre 1 e 100.")]
        public int PercentageOfExecutiveSeats { get; set; }
    }
}
