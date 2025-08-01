using FlyTickets2025.web.Data;
using FlyTickets2025.web.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.ValidationAttributes 
{
    public class UniqueFlightOnDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // 'value' is the FlightNumber
            if (value == null)
            {
                // If FlightNumber is null, let [Required] handle it
                return ValidationResult.Success;
            }

            string flightNumber = value.ToString() ?? string.Empty; // Cast to string

            // Get the Flight model instance being validated
            var flight = validationContext.ObjectInstance as FlightCreateViewModel; // Adjust type as needed
            if (flight == null)
            {
                return new ValidationResult("Validation context is not a Flight object.");
            }

            // Get DbContext from service provider
            var dbContext = validationContext.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                return new ValidationResult("Database context not available for validation.");
            }

            // Check for existing flights with the same FlightNumber on the same DepartureTime.Date
            var conflicts = dbContext.Flights
                .AsNoTracking()
                .Where(f => f.FlightNumber == flightNumber &&
                            f.DepartureTime.Date == flight.DepartureTime.Date &&
                            f.Id != flight.Id) // Exclude the current flight being edited
                .Any();

            if (conflicts)
            {
                return new ValidationResult(ErrorMessage ?? "A flight with this number already exists on the selected date.");
            }

            return ValidationResult.Success;
        }
    }
}
