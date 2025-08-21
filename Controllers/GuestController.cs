using Microsoft.AspNetCore.Mvc;
using HospitalityProject.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalityProject.Controllers
{
    public class GuestController : Controller
    {
        private readonly HotelDbContext _context;

        public GuestController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var guests = await _context.Guests.ToListAsync();
            return View(guests);
        }

        public IActionResult Create()
        {
            return View(); // Display form to create a new guest
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guest guest)
        {
            if (ModelState.IsValid)
            {
                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            return View(guest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Guest guest)
        {
            if (id != guest.GuestId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestId == id);
            if (guest == null) return NotFound();

            return View(guest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            return View(guest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
