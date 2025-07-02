using FlyTickets2025.web.Data.Entities; // For IServiceProvider (needed for GetService)
using FlyTickets2025.web.Repositories;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.ValidationAttributes // Adjust namespace
{
    public class NotBookedOnDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // This attribute is applied to AircraftId, so 'value' is the AircraftId
            if (value == null)
            {
                // If AircraftId is null, let [Required] handle it
                return ValidationResult.Success;
            }

            int aircraftId = (int)value;

            // Get the Flight model instance being validated
            var flight = validationContext.ObjectInstance as Flight;
            if (flight == null)
            {
                return new ValidationResult("Validation context is not a Flight object.");
            }

            // <<< CHANGE START: Get IAircraftRepository from service provider instead of DbContext
            var aircraftRepository = validationContext.GetService<IAircraftRepository>();
            if (aircraftRepository == null)
            {
                // This should not happen in a properly configured app where IAircraftRepository is registered
                return new ValidationResult("Aircraft repository not available for validation.");
            }

            // Call the new method on the repository
            // Use .GetAwaiter().GetResult() to synchronously wait for the async method in a non-async context.
            // This is necessary in ValidationAttribute.IsValid which is synchronous.
            bool conflicts = aircraftRepository.IsAircraftBookedOnDateAsync(
                aircraftId,
                flight.DepartureTime.Date, // Pass only the date part
                flight.Id // Pass the current flight's ID (for edit scenarios)
            ).GetAwaiter().GetResult(); // <<< CHANGE END

            if (conflicts)
            {
                // Use ErrorMessage property set on the attribute, or a default
                return new ValidationResult(ErrorMessage ?? "This aircraft is already assigned to another flight on the selected date.");
            }

            return ValidationResult.Success;
        }
    }
}