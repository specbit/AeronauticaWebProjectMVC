using FlyTickets2025.web.Data;
using FlyTickets2025.web.Data.Entities;
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
        public async Task<IActionResult> Create([Bind("Id,Model,TotalSeats,PhotoPath")] Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(aircraft);
                //await _context.SaveChangesAsync();

                await _aircraftRepository.CreateAsync(aircraft);
                await _aircraftRepository.SaveAllAsync(); 
                return RedirectToAction(nameof(Index));
            }
            return View(aircraft);
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
            return View(aircraft);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Model,TotalSeats,PhotoPath")] Aircraft aircraft)
        {
            if (id != aircraft.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(aircraft);
                    //await _context.SaveChangesAsync();
                    await _aircraftRepository.UpdateAsync(aircraft);
                    await _aircraftRepository.SaveAllAsync(); // Ensure changes are saved
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _aircraftRepository.ExistsAsync(aircraft.Id))
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
            return View(aircraft);
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
