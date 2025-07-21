// Controllers/FlightsController.cs
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // Still needed for DbUpdateConcurrencyException

namespace FlyTickets2025.web.Controllers
{
    public class FlightsController : Controller
    {
        //private readonly ApplicationDbContext _context; // Keep ApplicationDbContext for SelectLists for now, ideally move to specific repos later
        private readonly IFlightRepository _flightsRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ISeatRepository _seatRepository;  

        public FlightsController(
            IFlightRepository flightsRepository,
            IAircraftRepository aircraftRepository,
            ICityRepository cityRepository,
            ISeatRepository seatRepository)
        //ApplicationDbContext context)
        {
            //_context = context; // Keep context for SelectLists until dedicated City/Aircraft repositories for dropdowns
            _flightsRepository = flightsRepository;
            _aircraftRepository = aircraftRepository;
            _cityRepository = cityRepository;
            _seatRepository = seatRepository;
        }

        // GET: Flights
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> Index()
        {
            // Use repository method to get all flights including related entities
            var flights = await _flightsRepository.GetAllFlightsWithRelatedEntities().ToListAsync(); // <<< Use repository
            return View(flights);
        }

        [AllowAnonymous]
        public async Task<IActionResult> HomeCatalog(FlightSearchViewModel model) // Accept ViewModel
        {
            // Populate dropdowns for origin and destination cities
            var cities = await _cityRepository.GetAllCitiesAsync();
            model.OriginCities = new SelectList(cities, "Id", "Name", model.OriginCityId);
            model.DestinationCities = new SelectList(cities, "Id", "Name", model.DestinationCityId);

            // If search parameters are provided, perform the search
            if (model.DepartureDate.HasValue || model.OriginCityId.HasValue || model.DestinationCityId.HasValue)
            {
                model.AvailableFlights = await _flightsRepository.SearchFlightsAsync(
                    model.OriginCityId,
                    model.DestinationCityId,
                    model.DepartureDate);

                if (model.AvailableFlights != null && model.AvailableFlights.Any())
                {
                    foreach (var flight in model.AvailableFlights)
                    {
                        flight.SetEstimateArrival(); // Calculate estimated arrival time for each flight
                    }
                }
            }
            else
            {
                model.AvailableFlights = await _flightsRepository.SearchFlightsAsync(); // Start with an empty list by default

                if (model.AvailableFlights != null && model.AvailableFlights.Any())
                {
                    foreach (var flight in model.AvailableFlights)
                    {
                        flight.SetEstimateArrival(); // Calculate estimated arrival time for each flight
                    }
                }
            }

            return View(model); // Pass the populated ViewModel to the view
        }

        // GET: Flights/Details/5
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Use repository method to get a single flight including related entities
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id.Value); // <<< Use repository
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // GET: Flights/Create
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> Create()
        {
            // Keep direct context access for SelectLists for now
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model");
            ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName"); // Corrected to AirportName
            ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName"); // Corrected to AirportName
            return View();
        }

        // POST: Flights/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionário")]
        // Ensure all properties are in Bind attribute that come from the form
        public async Task<IActionResult> Create(FlightCreateViewModel request)
        {
            if (ModelState.IsValid)
            {
                Flight flight = new Flight
                {
                    FlightNumber = request.FlightNumber,
                    DepartureTime = request.DepartureTime,
                    DurationMinutes = request.DurationMinutes,
                    OriginCityId = request.OriginCityId,
                    DestinationCityId = request.DestinationCityId,
                    AircraftId = request.AircraftId,
                    DefaultFlightValue = request.DefaultFlightValue, // Assuming this is set in the ViewModel
                    PercentageOfExecutiveSeats = request.PercentageOfExecutiveSeats // Assuming this is set in the ViewModel
                };

                var newFlight = await _flightsRepository.CreateAsync(flight); 

                var seatsList = await _seatRepository.CreateSeatsForFlightAsync(newFlight.Id, request.DefaultFlightValue, request.PercentageOfExecutiveSeats);

                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBags if ModelState is invalid
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", request.AircraftId);
            ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", request.DestinationCityId);
            ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", request.OriginCityId);
            return View(request);
        }

        // GET: Flights/Edit/5
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Use repository method to get a single flight for editing
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id.Value); // <<< Use repository
            if (flight == null)
            {
                return NotFound();
            }
            // Repopulate ViewBags
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", flight.AircraftId);
            ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.DestinationCityId); // Corrected to AirportName
            ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.OriginCityId); // Corrected to AirportName
            return View(flight);
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionário")]
        // Ensure all properties are in Bind attribute that come from the form
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightNumber,DepartureTime,ArrivalTime,DurationMinutes,OriginCityId,DestinationCityId,AircraftId")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _flightsRepository.UpdateAsync(flight); // <<< Use repository
                    // SaveAllAsync is called internally by SuperShop-fashion repository
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Use repository's ExistsAsync
                    if (!await _flightsRepository.ExistsAsync(flight.Id)) // <<< Use repository
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Repopulate ViewBags if ModelState is invalid
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", flight.AircraftId);
            ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.DestinationCityId); // Corrected to AirportName
            ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.OriginCityId); // Corrected to AirportName
            return View(flight);
        }

        // GET: Flights/Delete/5
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Use repository method to get a single flight for deletion confirmation
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id.Value); // <<< Use repository
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _flightsRepository.GetByIdAsync(id);
            await _flightsRepository.DeleteAsync(flight); // <<< Use repository
            return RedirectToAction(nameof(Index));
        }
    }
}