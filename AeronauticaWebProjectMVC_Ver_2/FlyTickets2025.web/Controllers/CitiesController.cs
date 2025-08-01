using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Helpers;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CitiesController : Controller
    {
        private readonly ICityRepository _cityRepository;
        private readonly IConverterHelper _converterHelper;

        public CitiesController(ICityRepository cityRepository, IConverterHelper converterHelper)
        {
            _cityRepository = cityRepository;
            _converterHelper = converterHelper;
        }

        // GET: Cities
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Cities.ToListAsync());
            return View(await _cityRepository.GetAllAsync());
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var city = await _context.Cities.FirstOrDefaultAsync(m => m.Id == id);

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Cities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,AirportName,Country,FlagImagePath")] City city)
        public async Task<IActionResult> Create(CityViewModel cityViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                // Handle file upload if a file is provided
                if (cityViewModel.FlagImageFile != null && cityViewModel.FlagImageFile.Length > 0)
                {
                    // Get the directory to save the image
                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\cities", // Ensure the path is relative to wwwroot
                        cityViewModel.FlagImageFile.FileName);

                    // Save the uploaded file to the specified path
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await cityViewModel.FlagImageFile.CopyToAsync(stream);
                    }

                    // Set the FlagImagePath property to the relative path
                    path = $"~/images/cities/{cityViewModel.FlagImageFile.FileName}";
                }

                // Create a new City entity from the view model
                var city = _converterHelper.ToCityEntity(cityViewModel, path, true);

                //_context.Add(city);
                //await _context.SaveChangesAsync();
                await _cityRepository.CreateAsync(city);

                await _cityRepository.SaveAllAsync(); // Ensure changes are saved

                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the current model
            return View(cityViewModel);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var city = await _context.Cities.FindAsync(id);
            var city = await _cityRepository.GetByIdAsync(id.Value);

            if (city == null)
            {
                return NotFound();
            }

            // Convert City to CityViewModel for editing
            var cityViewModel = _converterHelper.ToCityViewModel(city);

            return View(cityViewModel);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CityViewModel cityViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the city entity that already exists in the database
                    var cityToUpdate = await _cityRepository.GetByIdAsync(cityViewModel.Id);

                    if (cityToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Get the existing path for the flag image
                    var path = cityToUpdate.FlagImagePath; // Keep the existing path

                    // Handle file upload if a new file is provided
                    if (cityViewModel.FlagImageFile != null && cityViewModel.FlagImageFile.Length > 0)
                    {
                        // Get the directory to save the image
                        var physicalPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\images\\cities", // Ensure the path is relative to wwwroot
                            cityViewModel.FlagImageFile.FileName);

                        // Save the uploaded file to the specified path
                        using (var stream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await cityViewModel.FlagImageFile.CopyToAsync(stream);
                        }

                        // Set the FlagImagePath property to the relative path
                        path = $"~/images/cities/{cityViewModel.FlagImageFile.FileName}";
                    }

                    var city = _converterHelper.ToCityEntity(cityViewModel, path!, true);

                    //_context.Update(city);
                    //await _context.SaveChangesAsync();

                    await _cityRepository.UpdateAsync(city);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cityRepository.ExistsAsync(cityViewModel.Id))
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
            return View(cityViewModel);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var city = await _context.Cities
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var city = await _cityRepository.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null) return NotFound();

            try
            {
                bool hasFlights = await _cityRepository.HasAssociatedFlightsAsync(id);
                if (hasFlights)
                {
                    TempData["SpecificErrorMessage"] = "It is not possible to delete this city because it has associated flights.";
                    return RedirectToAction("DeleteRestricted", "ErrorHandler");
                }

                await _cityRepository.DeleteAsync(city);
                await _cityRepository.SaveAllAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["SpecificErrorMessage"] = "It was not possible to delete the city due to a data restriction in the system.";
                return RedirectToAction("DeleteRestricted", "ErrorHandler");
            }
        }
    }
}
