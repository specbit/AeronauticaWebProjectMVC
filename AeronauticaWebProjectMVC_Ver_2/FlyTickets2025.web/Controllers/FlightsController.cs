// Controllers/FlightsController.cs
using FlyTickets2025.web.Data.Entities;
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
        private readonly IFlightsRepository _flightsRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly ICityRepository _cityRepository;

        public FlightsController(
            IFlightsRepository flightsRepository,
            IAircraftRepository aircraftRepository,
            ICityRepository cityRepository)
        //ApplicationDbContext context)
        {
            //_context = context; // Keep context for SelectLists until dedicated City/Aircraft repositories for dropdowns
            _flightsRepository = flightsRepository;
            _aircraftRepository = aircraftRepository;
            _cityRepository = cityRepository;
        }

        // GET: Flights
        [Authorize(Roles = "Funcionário")]
        public async Task<IActionResult> Index()
        {
            // Use repository method to get all flights including related entities
            var flights = await _flightsRepository.GetAllFlightsWithRelatedEntities().ToListAsync(); // <<< Use repository
            return View(flights);
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
        public async Task<IActionResult> Create([Bind("Id,FlightNumber,DepartureTime,ArrivalTime,DurationMinutes,OriginCityId,DestinationCityId,AircraftId")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                await _flightsRepository.CreateAsync(flight); // <<< Use repository
                // SaveAllAsync is called internally by SuperShop-fashion repository
                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBags if ModelState is invalid
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", flight.AircraftId);
            ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.DestinationCityId);
            ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.OriginCityId);
            return View(flight);
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