using FlyTickets2025.Web.Data.Entities;
using FlyTickets2025.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.Web.Controllers
{
    public class CitiesController : Controller
    {
        private readonly IGenericRepository<City> _cityRepository;

        public CitiesController(IGenericRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        // GET: Cities
        public async Task<IActionResult> Index()
        {
            return View(await _cityRepository.GetAllAsync());
        }

        // GET: Cities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("Id,Name,Country,FlagImagePath")] City city)
        {
            if (ModelState.IsValid)
            {
                await _cityRepository.CreateAsync(city);
                await _cityRepository.SaveAllAsync();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,FlagImagePath")] City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _cityRepository.UpdateAsync(city);
                    await _cityRepository.SaveAllAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _cityRepository.ExistsAsync(city.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Re-throw other unexpected exceptions
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
            if (city != null)
            {
                await _cityRepository.DeleteAsync(id);
            }

            await _cityRepository.SaveAllAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
