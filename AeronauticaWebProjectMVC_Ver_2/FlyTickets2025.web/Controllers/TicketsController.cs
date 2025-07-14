// REMOVE FlyTickets2025.web.Data; if no longer directly referenced besides entity types
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Helpers;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; // Still needed for DbUpdateConcurrencyException

namespace FlyTickets2025.web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IApplicationUserHelper _applicationUserHelper;       
        private readonly IFlightsRepository _flightsRepository;   // New
        private readonly ISeatsRepository _seatsRepository;       // New

        // Inject all necessary repositories
        public TicketsController(
            ITicketRepository ticketRepository,
        IApplicationUserHelper applicationUserHelper,
            IFlightsRepository flightRepository,
            ISeatsRepository seatRepository)
        {
            _ticketRepository = ticketRepository;
            _applicationUserHelper = applicationUserHelper;
            _flightsRepository = flightRepository;
            _seatsRepository = seatRepository;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            // Use the repository method that includes related entities
            var tickets = await _ticketRepository.GetAllWithUsersFlightsAndSeatsAsync();
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Use the repository method that includes related entities for a single item
            var ticket = await _ticketRepository.GetByIdWithUsersFlightsAndSeatsAsync(id.Value); // Use .Value for nullable int

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create(int? flightId)
        {
            var user = await _applicationUserHelper.GetUserByIdAsync(User);

            var ticket = new Ticket
            {
                ClientName = User.Identity?.Name,
                ApplicationUser = user,
                PurchaseDate = DateTime.Now,
                IsBooked = true,
                TicketPrice = 0.01M,
                PassengerType = PassengerType.Adult,
                FlightDate = DateTime.Now.Date,
                ApplicationUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty
            };

            // Dropdown de voos
            ViewBag.FlightId = new SelectList(
                await _flightsRepository.GetAllFlightsAsync(),
                "Id", "FlightNumber", flightId
            );

            // Dropdown de usuários
            ViewBag.ApplicationUserId = new SelectList(
                await _applicationUserHelper.GetAllUsersAsync(),
                "Id", "Email", ticket.ApplicationUserId
            );

            IEnumerable<Seat> seatsForDropdown;

            if (flightId.HasValue && flightId.Value > 0)
            {
                var selectedFlight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(flightId.Value);

                if (selectedFlight != null && selectedFlight.AircraftId > 0)
                {
                    seatsForDropdown = await _seatsRepository.GetSeatsByFlightIdAsync(selectedFlight.AircraftId);
                }
                else
                {
                    seatsForDropdown = await _seatsRepository.GetAllSeatsAsync();
                }

                ticket.FlightId = flightId.Value;
                ticket.FlightDate = selectedFlight?.DepartureTime.Date ?? DateTime.Now.Date;

                ViewBag.SelectedFlightDetails = selectedFlight;
            }
            else
            {
                seatsForDropdown = await _seatsRepository.GetAllSeatsAsync();
            }

            ViewBag.SeatId = new SelectList(seatsForDropdown, "Id", "SeatNumber");

            ViewBag.SeatPricesJson = JsonConvert.SerializeObject(
                seatsForDropdown.Select(s => new { id = s.Id, value = s.Value })
            );

            return View(ticket);
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FlightId,ApplicationUserId,SeatId,PassengerType,IsBooked,PurchaseDate,TicketPrice,ClientName,FlightDate")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                await _ticketRepository.AddAsync(ticket);
                return RedirectToAction(nameof(Index));
            }

            // Re-populate SelectLists if model state is invalid
            ViewData["ApplicationUserId"] = new SelectList(await _applicationUserHelper.GetAllUsersAsync(), "Id", "Id", ticket.ApplicationUserId);
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllFlightsAsync(), "Id", "FlightNumber", ticket.FlightId);
            ViewData["SeatId"] = new SelectList(await _seatsRepository.GetAllSeatsAsync(), "Id", "SeatNumber", ticket.SeatId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketRepository.GetByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            // Fetch data for SelectLists from their respective repositories
            ViewData["ApplicationUserId"] = new SelectList(await _applicationUserHelper.GetAllUsersAsync(), "Id", "Id", ticket.ApplicationUserId);
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllFlightsAsync(), "Id", "FlightNumber", ticket.FlightId);
            ViewData["SeatId"] = new SelectList(await _seatsRepository.GetAllSeatsAsync(), "Id", "SeatNumber", ticket.SeatId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightId,ApplicationUserId,SeatId,PassengerType,IsBooked,PurchaseDate,TicketPrice,ClientName,FlightDate")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketRepository.UpdateAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ticketRepository.TicketExistsAsync(ticket.Id))
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

            // Re-populate SelectLists if model state is invalid
            ViewData["ApplicationUserId"] = new SelectList(await _applicationUserHelper.GetAllUsersAsync(), "Id", "Id", ticket.ApplicationUserId);
            ViewData["FlightId"] = new SelectList(await _flightsRepository.GetAllFlightsAsync(), "Id", "FlightNumber", ticket.FlightId);
            ViewData["SeatId"] = new SelectList(await _seatsRepository.GetAllSeatsAsync(), "Id", "SeatNumber", ticket.SeatId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Use the repository method that includes related entities for display before deleting
            var ticket = await _ticketRepository.GetByIdWithUsersFlightsAndSeatsAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ticketRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}