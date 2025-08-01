using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Helpers;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.Net.Mail;
using Syncfusion.Drawing;
using System.Security.Claims;

namespace FlyTickets2025.web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IApplicationUserHelper _applicationUserHelper;
        private readonly IFlightRepository _flightsRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IEmailSender _emailSender;

        public TicketsController(
            ITicketRepository ticketRepository,
            IApplicationUserHelper applicationUserHelper,
            IFlightRepository flightRepository,
            ISeatRepository seatRepository,
            IEmailSender emailSender)
        {
            _ticketRepository = ticketRepository;
            _applicationUserHelper = applicationUserHelper;
            _flightsRepository = flightRepository;
            _seatRepository = seatRepository;
            _emailSender = emailSender;
        }

        // GET: Tickets (Your existing Index method)
        [Authorize(Roles = "Cliente,Funcionário")]
        public async Task<IActionResult> Index()
        {
            // Check if the user is authenticated.
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login if the user is not logged in.
                return RedirectToAction("Login", "Account");
            }

            // 1. Get the ID of the currently logged-in user.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Call the new repository method to get only the tickets for this specific user.
            var userTickets = await _ticketRepository.GetTicketsByUserIdAsync(userId);

            // 3. Pass the filtered list to the view.
            return View(userTickets);
        }
        //public async Task<IActionResult> Index()
        //{
        //    var tickets = await _ticketRepository.GetAllWithUsersFlightsAndSeatsAsync();
        //    return View(tickets);
        //}

        // GET: Tickets/Details/5 (Your existing Details method)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ticket = await _ticketRepository.GetByIdWithUsersFlightsAndSeatsAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // STEP 1: This method prepares the form for the user
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create(int flightId)
        {
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(flightId);
            if (flight == null)
            {
                return NotFound();
            }

            var viewModel = new TicketViewModel
            {
                FlightId = flightId,
                SelectedFlightDetails = flight,
                AvailableSeats = (await _seatRepository.GetSeatsByFlightIdAsync(flightId)).ToList(),
                CurrentUser = await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name)
            };

            return View(viewModel);
        }

        // STEP 2: This method processes the form and handles your business rule
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create(TicketViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.SelectedFlightDetails = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(viewModel.FlightId);
                viewModel.AvailableSeats = (await _seatRepository.GetSeatsByFlightIdAsync(viewModel.FlightId)).ToList();
                viewModel.CurrentUser = await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name);
                return View(viewModel);
            }

            var passengerUser = await _applicationUserHelper.GetUserByEmailasync(viewModel.PassengerEmail);

            if (passengerUser == null)
            {
                var names = viewModel.PassengerFullName.Split(new[] { ' ' }, 2);
                var firstName = names[0];
                var lastName = names.Length > 1 ? names[1] : "(No Last Name)";

                passengerUser = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = viewModel.PassengerEmail,
                    UserName = viewModel.PassengerEmail
                };

                var tempPassword = Guid.NewGuid().ToString("d").Substring(0, 8);
                var result = await _applicationUserHelper.AddUserAsync(passengerUser, tempPassword);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Could not create user account for the passenger.");
                    viewModel.SelectedFlightDetails = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(viewModel.FlightId);
                    viewModel.AvailableSeats = (await _seatRepository.GetSeatsByFlightIdAsync(viewModel.FlightId)).ToList();
                    viewModel.CurrentUser = await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name);
                    return View(viewModel);
                }
                await _applicationUserHelper.AddUserToRoleAsync(passengerUser, "Cliente");
            }

            // First, get the flight details so we have the correct date
            var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(viewModel.FlightId);

            var ticket = new Ticket
            {
                FlightId = viewModel.FlightId,
                FlightDate = flight.DepartureTime,
                SeatId = viewModel.SeatId,
                ApplicationUserId = passengerUser.Id,
                PassengerFullName = viewModel.PassengerFullName,
                DocumentType = viewModel.PassengerDocumentType,
                DocumentNumber = viewModel.PassengerDocumentNumber,
                PassengerType = viewModel.PassengerType,
                TicketPrice = viewModel.FinalPrice,
                ClientName = (await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name)).Email
            };

            ticket.Flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(ticket.FlightId);
            ticket.Seat = await _seatRepository.GetByIdAsync(ticket.SeatId);
            ticket.ApplicationUser = passengerUser;

            return View("~/Views/Payment/PaymentConfirmation.cshtml", ticket);
        }

        // STEP 3: This method finalizes the purchase after payment
        [HttpPost]
        [Authorize(Roles = "Cliente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentConfirmationDone(
    [Bind("FlightId,SeatId,ApplicationUserId,TicketPrice,PassengerType,PassengerFullName,DocumentType,DocumentNumber,ClientName")] Ticket ticket,
    string CardHolderName, string CardNumber, string ExpirationDate, string CVV)
        {
            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CVV))
            {
                TempData["ErrorMessage"] = "Invalid payment information provided.";
                return RedirectToAction("Create", new { flightId = ticket.FlightId });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var flight = await _flightsRepository.GetFlightWithRelatedEntitiesByIdAsync(ticket.FlightId);
                    var seat = await _seatRepository.GetByIdAsync(ticket.SeatId);
                    var passengerUser = await _applicationUserHelper.FindUserByIdAsync(ticket.ApplicationUserId);

                    ticket.FlightDate = flight.DepartureTime;
                    ticket.PurchaseDate = DateTime.Now;
                    ticket.IsBooked = true;

                    // --- Generate PDF ---
                    MemoryStream pdfStream = new MemoryStream();
                    using (PdfDocument document = new PdfDocument())
                    {
                        PdfPage page = document.Pages.Add();
                        PdfGraphics graphics = page.Graphics;

                        graphics.DrawString("Flight Ticket Confirmation", new PdfStandardFont(PdfFontFamily.Helvetica, 20), PdfBrushes.Blue, new PointF(150, 0));
                        graphics.DrawString($"Passenger: {ticket.PassengerFullName}", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new PointF(30, 40));
                        graphics.DrawString($"Flight: {flight.FlightNumber}", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new PointF(30, 60));
                        graphics.DrawString($"From: {flight.OriginCity.Name} To: {flight.DestinationCity.Name}", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new PointF(30, 80));
                        graphics.DrawString($"Departure: {flight.DepartureTime:g}", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new PointF(30, 100));
                        graphics.DrawString($"Seat: {seat.SeatNumber}", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new PointF(30, 120));

                        document.Save(pdfStream);
                    }
                    pdfStream.Position = 0;

                    // --- Send Email with PDF Attachment ---
                    var pdfAttachment = new Attachment(pdfStream, $"Ticket-{flight.FlightNumber}.pdf", "application/pdf");
                    await _emailSender.SendEmailAsync(passengerUser.Email,
                        $"Your Ticket for flight {flight.FlightNumber} is Confirmed!",
                        "Thank you for your purchase. Please find your ticket attached.",
                        pdfAttachment);

                    // --- Save to Database ---
                    await _ticketRepository.CreateAsync(ticket);
                    seat.IsAvailableForSale = false;
                    await _seatRepository.UpdateAsync(seat);
                    await _ticketRepository.SaveAllAsync();

                    return RedirectToAction("PurchaseSuccess", new { id = ticket.Id });
                }
                catch (Exception ex)
                {
                    // Optional: Log the exception 'ex' to a file for debugging
                    TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                    return RedirectToAction("Create", new { flightId = ticket.FlightId });
                }
            }
            return RedirectToAction("Create", new { flightId = ticket.FlightId });
        }
        //public async Task<IActionResult> PaymentConfirmationDone(
        //    [Bind("FlightId,SeatId,ApplicationUserId,TicketPrice,PassengerType,PassengerFullName,DocumentType,DocumentNumber,ClientName")] Ticket ticket,
        //    string CardHolderName, string CardNumber, string ExpirationDate, string CVV)
        //{
        //    if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CVV))
        //    {
        //        TempData["ErrorMessage"] = "Invalid payment information provided.";
        //        return RedirectToAction("Create", new { flightId = ticket.FlightId });
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var flight = await _flightsRepository.GetByIdAsync(ticket.FlightId);
        //            ticket.FlightDate = flight.DepartureTime;
        //            ticket.PurchaseDate = DateTime.Now;
        //            ticket.IsBooked = true;

        //            await _ticketRepository.CreateAsync(ticket);

        //            var seat = await _seatsRepository.GetByIdAsync(ticket.SeatId);
        //            if (seat != null)
        //            {
        //                seat.IsAvailableForSale = false;
        //                await _seatsRepository.UpdateAsync(seat);
        //            }

        //            await _ticketRepository.SaveAllAsync();

        //            return RedirectToAction("PurchaseSuccess", new { id = ticket.Id });
        //        }
        //        catch (Exception)
        //        {
        //            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
        //            return RedirectToAction("Create", new { flightId = ticket.FlightId });
        //        }
        //    }
        //    return RedirectToAction("Create", new { flightId = ticket.FlightId });
        //}

        // STEP 4: This is the success page after a purchase
        public async Task<IActionResult> PurchaseSuccess(int id)
        {
            var ticket = await _ticketRepository.GetByIdWithUsersFlightsAndSeatsAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }
    }
}