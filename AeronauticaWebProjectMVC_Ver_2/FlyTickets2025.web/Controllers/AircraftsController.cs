using FlyTickets2025.web.Helpers;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AircraftsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IAircraftRepository _aircraftRepository;
        private readonly IConverterHelper _converterHelper;

        public AircraftsController(IAircraftRepository aircraftRepository, IConverterHelper converterHelper)
        {
            //_context = context;
            _aircraftRepository = aircraftRepository;
            _converterHelper = converterHelper;
        }

        // GET: Aircrafts
        public async Task<IActionResult> Index()
        {
            return View(await _aircraftRepository.GetAllAsync()); // If using repository pattern
        }

        // GET: Aircrafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var aircraft = await _context.Aircrafts
            //    .FirstOrDefaultAsync(m => m.Id == id);

            // Using repository pattern, replace the above line with:
            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return NotFound();
            }

            return View(aircraft);
        }

        // GET: Aircrafts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aircrafts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AircraftViewModel aircraftViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                // Handle file upload if a file is provided
                if (aircraftViewModel.AircraftImageFile != null && aircraftViewModel.AircraftImageFile.Length > 0)
                {
                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\aircrafts", // Ensure the path is relative to wwwroot
                        aircraftViewModel.AircraftImageFile.FileName);

                    // Save the uploaded file to the specified path
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await aircraftViewModel.AircraftImageFile.CopyToAsync(stream);
                    }

                    // Set the FlagImagePath property to the relative path
                    path = $"~/images/aircrafts/{aircraftViewModel.AircraftImageFile.FileName}";
                }

                // Create a new Aircraft entity from the view model
                var aircraft = _converterHelper.ToAircraftEntity(aircraftViewModel, path, true);

                await _aircraftRepository.CreateAsync(aircraft);
                await _aircraftRepository.SaveAllAsync(); // Ensure changes are saved

                return RedirectToAction(nameof(Index));
            }
            return View(aircraftViewModel);
        }

        // GET: Aircrafts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var aircraft = await _context.Aircrafts.FindAsync(id);

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return NotFound();
            }

            // Convert the Aircraft entity to AircraftViewModel for editing
            var aircraftViewModel = _converterHelper.ToAircraftViewModel(aircraft);

            // Check if the aircraft has tickets and pass this info to the view.
            ViewBag.HasSoldTickets = await _aircraftRepository.HasSoldTicketsAsync(id.Value);

            return View(aircraftViewModel);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AircraftViewModel aircraftViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Get the aircraft entity that already exists in the database
                    var aircraftToUpdate = await _aircraftRepository.GetByIdAsync(aircraftViewModel.Id);

                    bool hasTickets = await _aircraftRepository.HasSoldTicketsAsync(aircraftViewModel.Id);

                    if (aircraftToUpdate == null)
                    {
                        return NotFound();
                    }

                    // If it has tickets, AND the user tried to change the seat count
                    if (hasTickets && aircraftToUpdate.TotalSeats != aircraftViewModel.TotalSeats)
                    {
                        ModelState.AddModelError("TotalSeats", "Cannot change seat count on an aircraft with sold tickets.");
                        ViewBag.HasSoldTickets = true; // Pass the flag back to the view
                        return View(aircraftViewModel);
                    }

                    // 2. Get the existing path for the aircraft image
                    var path = aircraftToUpdate.AircraftImagePath;

                    // 3. Handle file upload if a new file is provided
                    if (aircraftViewModel.AircraftImageFile != null && aircraftViewModel.AircraftImageFile.Length > 0)
                    {
                        // Get the physical path to save the image
                        var physicalPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\aircrafts",
                            aircraftViewModel.AircraftImageFile.FileName);

                        // Save the uploaded file
                        using (var stream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await aircraftViewModel.AircraftImageFile.CopyToAsync(stream);
                        }

                        // Update the path variable with the new relative path
                        path = $"~/images/aircrafts/{aircraftViewModel.AircraftImageFile.FileName}";
                    }

                    // 4. Update the properties on the entity you fetched
                    aircraftToUpdate.Model = aircraftViewModel.Model;
                    aircraftToUpdate.TotalSeats = aircraftViewModel.TotalSeats;
                    aircraftToUpdate.AircraftImagePath = path; // Use the path (either old or new)

                    // 5. Save the updated entity
                    await _aircraftRepository.UpdateAsync(aircraftToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aircraftRepository.ExistsAsync(aircraftViewModel.Id))
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

            // If model state is invalid, return the view with the submitted data
            return View(aircraftViewModel);
        }

        // GET: Aircrafts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var aircraft = await _context.Aircrafts
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var aircraft = await _aircraftRepository.GetByIdAsync(id.Value);
            if (aircraft == null)
            {
                return NotFound();
            }

            // Check for tickets and pass the flag to the view.
            ViewBag.HasSoldTickets = await _aircraftRepository.HasSoldTicketsAsync(id.Value);

            return View(aircraft);
        }

        // POST: Aircrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aircraft = await _aircraftRepository.GetByIdAsync(id);

            if (aircraft == null)
            {
                return NotFound();
            }

            try
            {
                // --- Business Rule Check: Check if the aircraft has any associated flights ---
                bool hasFlights = await _aircraftRepository.HasAssociatedFlightsAsync(id);
                if (hasFlights)
                {
                    // Set a specific message for this scenario
                    TempData["SpecificErrorMessage"] = "Não é possível apagar esta aeronave porque tem voos agendados.";
                    // Redirect to the new, specific page for delete restrictions
                    return RedirectToAction("DeleteRestricted", "ErrorHandler");
                }
                // --- End Business Rule Check ---

                // If the check passes, get the aircraft and proceed with deletion
                await _aircraftRepository.DeleteAsync(aircraft);
                //await _aircraftRepository.SaveAllAsync(); // Ensure changes are saved

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex) // Catches database constraint violations if the above check was bypassed or failed
            {
                // Log the full exception for debugging (VERY IMPORTANT in a real app)
                // _logger.LogError(ex, "Error deleting aircraft {AircraftId} due to DB constraint.", id);

                TempData["SpecificErrorMessage"] = "It was not possible to delete the aircraft due to a data restriction in the system.";
                return RedirectToAction("DeleteRestricted", "ErrorHandler");
            }
            catch (Exception ex) // Catch any other truly unexpected errors
            {
                // Log the exception
                // _logger.LogError(ex, "An unexpected error occurred while deleting aircraft {AircraftId}.", id);

                TempData["CustomErrorMessage"] = "An unexpected error occurred while deleting the aircraft. Please, try again.";
                return RedirectToAction("Error", "ErrorHandler"); // Redirect to the GENERIC 500 error page
            }
        }
    }
}
