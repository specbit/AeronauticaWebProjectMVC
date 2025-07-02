using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
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
        public async Task<IActionResult> Create([Bind("Id,Name,AirportName,Country,FlagImagePath")] City city)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(city);
                //await _context.SaveChangesAsync();
                await _cityRepository.CreateAsync(city);
                await _cityRepository.SaveAllAsync(); // Ensure changes are saved
                return RedirectToAction(nameof(Index));
            }
            return View(city);
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
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AirportName,Country,FlagImagePath")] City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(city);
                    //await _context.SaveChangesAsync();

                    await _cityRepository.UpdateAsync(city);
                    await _cityRepository.SaveAllAsync(); // Ensure changes are saved
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cityRepository.ExistsAsync(city.Id))
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
            return View(city);
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
