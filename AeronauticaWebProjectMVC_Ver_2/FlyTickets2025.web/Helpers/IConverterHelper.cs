using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;

namespace FlyTickets2025.web.Helpers
{
    public interface IConverterHelper
    {
        // --- Aircraft ---
        Aircraft ToAircraftEntity(AircraftViewModel model, string path, bool isNew);
        AircraftViewModel ToAircraftViewModel(Aircraft aircraft);

        // --- City ---
        City ToCityEntity(CityViewModel model, string path, bool isNew);
        CityViewModel ToCityViewModel(City city);
    }
}

