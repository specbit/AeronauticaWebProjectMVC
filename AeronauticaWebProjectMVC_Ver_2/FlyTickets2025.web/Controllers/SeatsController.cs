using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FlyTickets2025.web.Controllers
{
    public class SeatsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly ISeatsRepository _seatsRepository;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IFlightsRepository _flightsRepository;


        public SeatsController(
            ISeatsRepository seatsRepository, 
            IAircraftRepository aircraftRepository, 
            IFlightsRepository flightsRepository)
        {
            //_context = context;
            _seatsRepository = seatsRepository;
            _aircraftRepository = aircraftRepository;
            _flightsRepository = flightsRepository;
        }

        // GET: Seats
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Seats.Include(s => s.AircraftModel).Include(s => s.Flight);
            //return View(await applicationDbContext.ToListAsync());

            return View(await _seatsRepository.GetAllSeatsWithRelatedEntitiesAsync()); // Using repository pattern
        }

        // GET: Seats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var seat = await _context.Seats
            //    .Include(s => s.AircraftModel)
            //    .Include(s => s.Flight)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            // Using repository pattern, replace the above line with:
            var seat = await _seatsRepository.GetSeatWithRelatedEntitiesByIdAsync(id.Value);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }

        // GET: Seats/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model");
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllAsync(), "Id", "FlightNumber");
            ViewData["SeatType"] = new SelectList(Enum.GetValues(typeof(SeatClass))
                                                        .Cast<SeatClass>()
                                                        .Select(e => new SelectListItem
                                                        {
                                                            Value = e.ToString(), // Store the enum name (e.g., "Economy")
                                                            Text = e.GetType()
                                                                    .GetMember(e.ToString())
                                                                    .First()
                                                                    .GetCustomAttribute<DisplayAttribute>()?
                                                                    .Name ?? e.ToString() // Use DisplayAttribute Name or fallback to enum name
                                                        }), "Value", "Text");
            return View();
        }

        // POST: Seats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeatNumber,Type,Value,IsAvailableForSale,FlightId,AircraftId")] Seat seat)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(seat);
                //await _context.SaveChangesAsync();

                // Using repository pattern, replace the above lines with:
                seat.IsAvailableForSale = true; // Ensure the seat is available for sale by default
                await _seatsRepository.CreateAsync(seat);
                //await _seatsRepository.SaveAllAsync(); // No need to call SaveAllAsync here, as CreateAsync should handle it internally

                return RedirectToAction(nameof(Index));
            }
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", seat.AircraftId);
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllAsync(), "Id", "FlightNumber", seat.FlightId);
            return View(seat);
        }

        // TODO: Make sure that a flight change is not allowed if there are tickets already booked for this seat.
        // GET: Seats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var seat = await _context.Seats.FindAsync(id);
            // Using repository pattern, replace the above line with:
            var seat = await _seatsRepository.GetSeatWithRelatedEntitiesByIdAsync(id.Value);
            if (seat == null)
            {
                return NotFound();
            }
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", seat.AircraftId);
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllAsync(), "Id", "FlightNumber", seat.FlightId);
            ViewData["SeatType"] = new SelectList(Enum.GetValues(typeof(SeatClass))
                                                        .Cast<SeatClass>()
                                                        .Select(e => new SelectListItem
                                                        {
                                                            Value = e.ToString(), // Store the enum name (e.g., "Economy")
                                                            Text = e.GetType()
                                                                    .GetMember(e.ToString())
                                                                    .First()
                                                                    .GetCustomAttribute<DisplayAttribute>()?
                                                                    .Name ?? e.ToString() // Use DisplayAttribute Name or fallback to enum name
                                                        }), "Value", "Text");
            return View(seat);
        }

        // POST: Seats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeatNumber,Type,Value,IsAvailableForSale,FlightId,AircraftId")] Seat seat)
        {
            if (id != seat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(seat);
                    //await _context.SaveChangesAsync();

                    // Using repository pattern, replace the above lines with:
                    seat.IsAvailableForSale = true; // Ensure the seat is available for sale by default
                    await _seatsRepository.UpdateAsync(seat);
                    //await _seatsRepository.SaveAllAsync(); // No need to call SaveAllAsync here, as UpdateAsync should handle it internally
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _seatsRepository.ExistsAsync(seat.Id))
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
            ViewData["AircraftId"] = new SelectList(await _aircraftRepository.GetAllAsync(), "Id", "Model", seat.AircraftId);
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllAsync(), "Id", "FlightNumber", seat.FlightId);
            ViewData["SeatType"] = new SelectList(Enum.GetValues(typeof(SeatClass))
                                                        .Cast<SeatClass>()
                                                        .Select(e => new SelectListItem
                                                        {
                                                            Value = e.ToString(),
                                                            Text = e.GetType()
                                                                    .GetMember(e.ToString())
                                                                    .First()
                                                                    .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                                                        }), "Value", "Text", seat.Type);


            return View(seat);
        }

        // GET: Seats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var seat = await _context.Seats
            //    .Include(s => s.AircraftModel)
            //    .Include(s => s.Flight)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            // Using repository pattern, replace the above line with:
            var seat = await _seatsRepository.GetSeatWithRelatedEntitiesByIdAsync(id.Value);
            if (seat == null)
            {
                return NotFound();
            }

            return View(seat);
        }

        // POST: Seats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var seat = await _context.Seats.FindAsync(id);
            //if (seat != null)
            //{
            //    _context.Seats.Remove(seat);
            //}

            //await _context.SaveChangesAsync();
            var seat = await _seatsRepository.GetByIdAsync(id);
            if (seat != null)
            {
                await _seatsRepository.DeleteAsync(seat);
                //await _seatsRepository.SaveAllAsync(); // No need to call SaveAllAsync here, as DeleteAsync should handle it internally
            }

            return RedirectToAction(nameof(Index));
        }

        // This action will be called by JavaScript to get the AircraftId and Model for a selected FlightId
        [HttpGet] // This specifies it responds to HTTP GET requests
        public async Task<IActionResult> GetAircraftByFlightId(int flightId)
        {
            // Use the FlightsRepository to get the Flight with its associated Aircraft eagerly loaded
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(flightId);

            if (flight == null)
            {
                // If the flight is not found, return JSON with nulls to indicate no aircraft.
                // This allows the JavaScript success handler to still run gracefully.
                return Json(new { aircraftId = (int?)null, aircraftModel = "N/A" });
            }

            // Return the AircraftId and Model Name as JSON.
            // Use null-conditional operator '?.' for safety in case flight.Aircraft is somehow null
            // (e.g., if GetFlightWithRelatedEntitiesByIdAsync didn't include it, or data is inconsistent).
            return Json(new { aircraftId = flight.Aircraft?.Id, aircraftModel = flight.Aircraft?.Model });
        }

        //private bool SeatExists(int id)
        //{
        //    return _context.Seats.Any(e => e.Id == id);
        //}
    }
}
