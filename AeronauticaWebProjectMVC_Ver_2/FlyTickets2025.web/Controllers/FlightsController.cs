using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Controllers
{
    public class FlightsController : Controller
    {
        private readonly IFlightRepository _flightsRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ISeatRepository _seatRepository;

        public FlightsController(
            IFlightRepository flightsRepository,
            IAircraftRepository aircraftRepository,
            ICityRepository cityRepository,
            ISeatRepository seatRepository)
        {
            _flightsRepository = flightsRepository;
            _aircraftRepository = aircraftRepository;
            _cityRepository = cityRepository;
            _seatRepository = seatRepository;
        }

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
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id.Value);

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
                // --- Start Validation Block ---
                var arrivalTime = request.DepartureTime.AddMinutes(request.DurationMinutes);
                bool isBooked = await _flightsRepository.IsAircraftBookedForPeriodAsync(request.AircraftId, request.DepartureTime, arrivalTime);

                if (isBooked)
                {
                    // If booked, add a model error and return to the form.
                    ModelState.AddModelError("AircraftId", "This aircraft is already scheduled for another flight during this time.");

                    ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", request.AircraftId);
                    ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", request.DestinationCityId);
                    ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", request.OriginCityId);
                    return View(request);
                }
                // --- End Validation Block ---

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

                // SaveAllAsync is called internally by Generic-fashion repository
                //Save the flight to the database to get its ID
                await _flightsRepository.SaveAllAsync();

                var seatsList = await _seatRepository.CreateSeatsForFlightAsync(newFlight.Id, request.DefaultFlightValue, request.PercentageOfExecutiveSeats);

                return RedirectToAction(nameof(Index));
            }

            // Repopulate ViewBags if ModelState is invalid
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", request.AircraftId);
            ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", request.DestinationCityId);
            ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", request.OriginCityId);
            return View(request);
        }

        #region comm
        //// GET: Flights/Edit/5
        //[Authorize(Roles = "Funcionário")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    // Use repository method to get a single flight for editing
        //    var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id.Value);

        //    if (flight == null)
        //    {
        //        return NotFound();
        //    }

        //    // Method to get only valid aircraft for the dropdown.
        //    var availableAircraft = await _flightsRepository.GetAvailableAircraftForFlightAsync(id.Value);

        //    // Repopulate ViewBags
        //    ViewData["AircraftId"] = new SelectList(availableAircraft, "Id", "Model", flight.AircraftId);
        //    ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.DestinationCityId); // Corrected to AirportName
        //    ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.OriginCityId); // Corrected to AirportName
        //    return View(flight);
        //}

        //// POST: Flights/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Funcionário")]
        //// Ensure all properties are in Bind attribute that come from the form
        //public async Task<IActionResult> Edit(int id, [Bind("Id,FlightNumber,DepartureTime,ArrivalTime,DurationMinutes,OriginCityId,DestinationCityId,AircraftId")] Flight flight)
        //{
        //    if (id != flight.Id)
        //    {
        //        return NotFound();
        //    }

        //    // Server-side validation to ensure a valid aircraft was submitted.
        //    var availableAircraft = await _flightsRepository.GetAvailableAircraftForFlightAsync(id);

        //    if (!availableAircraft.Any(a => a.Id == flight.AircraftId))
        //    {
        //        ModelState.AddModelError("AircraftId", "The selected aircraft is not available (check seat capacity or schedule).");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _flightsRepository.UpdateAsync(flight);
        //            // SaveAllAsync is called internally by Generic-fashion repository
        //            await _flightsRepository.SaveAllAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            // Use repository's ExistsAsync
        //            if (!await _flightsRepository.ExistsAsync(flight.Id)) 
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    // Repopulate ViewBags if ModelState is invalid
        //    ViewData["AircraftId"] = new SelectList(availableAircraft, "Id", "Model", flight.AircraftId);
        //    ViewData["DestinationCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.DestinationCityId); // Corrected to AirportName
        //    ViewData["OriginCityId"] = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.OriginCityId); // Corrected to AirportName
        //    return View(flight);
        //}
        #endregion

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var flight = await _flightsRepository.GetByIdAsync(id.Value);
            if (flight == null) return NotFound();

            // Map the database entity to the new ViewModel
            var model = new FlightEditViewModel
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                DepartureTime = flight.DepartureTime,
                DurationMinutes = flight.DurationMinutes,
                OriginCityId = flight.OriginCityId,
                DestinationCityId = flight.DestinationCityId,
                AircraftId = flight.AircraftId
            };

            // Populate the SelectLists on the ViewModel
            //var availableAircraft = await _flightsRepository.GetAvailableAircraftForFlightAsync(id.Value);

            // Ensure the currently assigned aircraft is included
            var availableAircraft = (await _flightsRepository.GetAvailableAircraftForFlightAsync(id.Value)).ToList();
            // Ensure the current aircraft is in the dropdown
            if (!availableAircraft.Any(a => a.Id == flight.AircraftId))
            {
                var assignedAircraft = await _aircraftRepository.GetByIdAsync(flight.AircraftId);
                if (assignedAircraft != null)
                {
                    availableAircraft.Add(assignedAircraft);
                }
            }
            model.AircraftList = new SelectList(availableAircraft, "Id", "Model", flight.AircraftId);
            model.DestinationCityList = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.DestinationCityId);
            model.OriginCityList = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", flight.OriginCityId);

            return View(model);
        }

        // POST: Flights/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlightEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            // Re-run the server-side validation for the selected aircraft
            var availableAircraft = await _flightsRepository.GetAvailableAircraftForFlightAsync(id);
            if (!availableAircraft.Any(a => a.Id == model.AircraftId))
            {
                ModelState.AddModelError("AircraftId", "The selected aircraft is not available (check seat capacity or schedule).");
            }

            if (ModelState.IsValid)
            {
                // Map the ViewModel back to the database entity
                var flightToUpdate = await _flightsRepository.GetByIdAsync(id);
                flightToUpdate.FlightNumber = model.FlightNumber;
                flightToUpdate.DepartureTime = model.DepartureTime;
                flightToUpdate.DurationMinutes = model.DurationMinutes;
                flightToUpdate.OriginCityId = model.OriginCityId;
                flightToUpdate.DestinationCityId = model.DestinationCityId;
                flightToUpdate.AircraftId = model.AircraftId;

                flightToUpdate.SetEstimateArrival(); // Recalculate ArrivalTime

                await _flightsRepository.UpdateAsync(flightToUpdate);
                await _flightsRepository.SaveAllAsync();
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate the dropdowns and return the view
            model.AircraftList = new SelectList(availableAircraft, "Id", "Model", model.AircraftId);
            model.DestinationCityList = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", model.DestinationCityId);
            model.OriginCityList = new SelectList(await _cityRepository.GetAllAsync(), "Id", "AirportName", model.OriginCityId);

            return View(model);
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
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id.Value); 
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
            //var flight = await _flightsRepository.GetByIdAsync(id);
            //await _flightsRepository.DeleteAsync(flight); 

            /*--------------------------------------*/

            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(id);

            if (flight == null)
            {
                return NotFound(); // Flight not found, either deleted or invalid ID
            }

            // --- Business Rule Check: Prevent delete if flight has associated tickets sold ---
            bool hasTicketsSold = await _flightsRepository.HasAssociatedTicketsOrSeatsAsync(id);

            if (hasTicketsSold)
            {
                // Add a model error to display on the Delete view
                ModelState.AddModelError(string.Empty, "Não é possível apagar este voo porque tem bilhetes associados ou lugares vendidos.");

                // Re-render the Delete view with the flight object and the error message
                return View("Delete", flight);
            }
            // --- End Business Rule Check ---


            // --- MANUAL DELETION OF DEPENDENT SEATS (Due to "No Cascade Delete" Rule) ---
            // If we reach this point, it means no tickets are sold for this flight.
            // However, the flight will still have its pre-generated Seats.
            // Since we cannot cascade delete, we MUST delete these Seats FIRST.
            // The database will allow deleting Seats IF they do NOT have associated Tickets (which we've already checked).

            if (flight.Seats != null && flight.Seats.Any())
            {
                // Create a copy of the collection to avoid "collection modified" errors during iteration and deletion.
                var seatsToDelete = flight.Seats.ToList();
                foreach (var seat in seatsToDelete)
                {
                    await _seatRepository.DeleteAsync(seat);
                }
                await _seatRepository.SaveAllAsync(); // Persist the seat deletions
            }
            // --- End Manual Seat Deletion ---

            // Now, after all dependent seats (without tickets) are removed, delete the flight itself.
            await _flightsRepository.DeleteAsync(flight);

            /*-------------------------------*/
            return RedirectToAction(nameof(Index));
        }
    }
}