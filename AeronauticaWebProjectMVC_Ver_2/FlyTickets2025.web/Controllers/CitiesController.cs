using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;
using FlyTickets2025.web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Controllers
{
    public class CitiesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly ICityRepository _cityRepository;

        public CitiesController(ICityRepository cityRepository)
        {
            //_context = context;
            _cityRepository = cityRepository;
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
                var city = this.ToCityEntity(cityViewModel, path);

                //_context.Add(city);
                //await _context.SaveChangesAsync();
                await _cityRepository.CreateAsync(city);
                await _cityRepository.SaveAllAsync(); // Ensure changes are saved
                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, return the view with the current model
            return View(cityViewModel);
        }

        private City ToCityEntity(CityViewModel cityViewModel, string path)
        {
            return new City
            {
                Id = cityViewModel.Id,
                Name = cityViewModel.Name,
                AirportName = cityViewModel.AirportName,
                Country = cityViewModel.Country,
                FlagImagePath = path // Set the path to the uploaded image
            };
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
            var cityViewModel= ToCityViewModel(city);

            return View(cityViewModel);
        }

        private static CityViewModel? ToCityViewModel(City city)
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

                    var city = this.ToCityEntity(cityViewModel, path!);

                    //_context.Update(city);
                    //await _context.SaveChangesAsync();

                    await _cityRepository.UpdateAsync(city);
                    await _cityRepository.SaveAllAsync(); // Ensure changes are saved
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

            await _cityRepository.DeleteAsync(city);
            await _cityRepository.SaveAllAsync(); // Ensure changes are saved

            //if (city != null)
            //{
            //    _context.Cities.Remove(city);
            //}

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool CityExists(int id)
        //{
        //    return _context.Cities.Any(e => e.Id == id);
        //}
    }
}
