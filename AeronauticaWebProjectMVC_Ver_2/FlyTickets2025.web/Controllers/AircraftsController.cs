using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Controllers
{
    public class AircraftsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IAircraftRepository _aircraftRepository;

        public AircraftsController(IAircraftRepository aircraftRepository)
        {
            //_context = context;
            _aircraftRepository = aircraftRepository;
        }

        // GET: Aircrafts
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Aircrafts.ToListAsync());
            return View(await _aircraftRepository.GetAllAsync()); // If using repository pattern
        }

        // GET: Aircrafts/Details/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aircrafts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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
                var aircraft = this.ToAircraftEntity(aircraftViewModel, path);

                //_context.Add(aircraft);
                //await _context.SaveChangesAsync();

                await _aircraftRepository.CreateAsync(aircraft);
                await _aircraftRepository.SaveAllAsync(); 
                return RedirectToAction(nameof(Index));
            }
            return View(aircraftViewModel);
        }

        private Aircraft ToAircraftEntity(AircraftViewModel aircraftViewModel, string path)
        {
            return new Aircraft
            {
                Model = aircraftViewModel.Model,
                TotalSeats = aircraftViewModel.TotalSeats,
                AircraftImagePath = path // Use the path for the image
            };
        }

        // GET: Aircrafts/Edit/5
        [Authorize(Roles = "Administrador")]
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
            var aircraftViewModel = this.ToAircraftViewModel(aircraft);

            return View(aircraftViewModel);
        }

        private AircraftViewModel ToAircraftViewModel(Aircraft aircraft)
        {
            return new AircraftViewModel
            {
                Id = aircraft.Id,
                Model = aircraft.Model,
                TotalSeats = aircraft.TotalSeats,
                //AircraftImageFile = null, // No file uploaded yet
                AircraftImagePath = aircraft.AircraftImagePath // Keep the existing image path
            };
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(AircraftViewModel aircraftViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Get the aircraft entity that already exists in the database
                    var aircraftToUpdate = await _aircraftRepository.GetByIdAsync(aircraftViewModel.Id);
                    if (aircraftToUpdate == null)
                    {
                        return NotFound();
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
                    await _aircraftRepository.SaveAllAsync();
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
        [Authorize(Roles = "Administrador")]
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

            return View(aircraft);
        }

        // POST: Aircrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var aircraft = await _context.Aircrafts.FindAsync(id);
            //if (aircraft != null)
            //{
            //    _context.Aircrafts.Remove(aircraft);
            //}

            //await _context.SaveChangesAsync();
            var aircraft = await _aircraftRepository.GetByIdAsync(id);
            if (aircraft != null)
            {
                await _aircraftRepository.DeleteAsync(aircraft);
                await _aircraftRepository.SaveAllAsync(); // Ensure changes are saved
            }
            return RedirectToAction(nameof(Index));
        }

        //private bool AircraftExists(int id)
        //{
        //    return _context.Aircrafts.Any(e => e.Id == id);
        //}
    }
}
