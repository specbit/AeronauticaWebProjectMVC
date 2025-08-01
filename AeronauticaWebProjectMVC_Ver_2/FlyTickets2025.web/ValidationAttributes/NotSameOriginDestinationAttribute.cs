using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.Web.ValidationAttributes 
{
    public class NotSameOriginDestinationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // This attribute can be applied to either OriginCityId or DestinationCityId.
            // We need the full Flight object to compare both.
            var flight = validationContext.ObjectInstance as FlightCreateViewModel;

            if (flight == null)
            {
                return new ValidationResult("Validation context is not a Flight object.");
            }

            // If either ID is zero (not selected in dropdowns yet, common during initial form load),
            // or if they are the same, validation fails.
            // We also check against 0 as often 0 is the default unselected value for int dropdowns.
            if (flight.OriginCityId == 0 || flight.DestinationCityId == 0 || flight.OriginCityId == flight.DestinationCityId)
            {
                return new ValidationResult(ErrorMessage ?? "Origin and Destiny can't be the same.");
            }

            return ValidationResult.Success;
        }
    }
}
