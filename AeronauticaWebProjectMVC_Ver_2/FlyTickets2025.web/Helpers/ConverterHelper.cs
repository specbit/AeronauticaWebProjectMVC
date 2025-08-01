using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;

namespace FlyTickets2025.web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Aircraft ToAircraftEntity(AircraftViewModel model, string path, bool isNew)
        {
            return new Aircraft
            {
                Id = isNew ? 0 : model.Id,
                Model = model.Model,
                TotalSeats = model.TotalSeats,
                AircraftImagePath = path
            };
        }

        public AircraftViewModel ToAircraftViewModel(Aircraft aircraft)
        {
            return new AircraftViewModel
            {
                Id = aircraft.Id,
                Model = aircraft.Model,
                TotalSeats = aircraft.TotalSeats,
                AircraftImagePath = aircraft.AircraftImagePath
            };
        }

        public City ToCityEntity(CityViewModel model, string path, bool isNew)
        {
            return new City
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                AirportName = model.AirportName,
                Country = model.Country,
                FlagImagePath = path
            };
        }

        public CityViewModel ToCityViewModel(City city)
        {
            return new CityViewModel
            {
                Id = city.Id,
                Name = city.Name,
                AirportName = city.AirportName,
                Country = city.Country,
                FlagImagePath = city.FlagImagePath
            };
        }
    }
}
