using Microsoft.AspNetCore.Mvc;
using HospitalityProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalityProject.Controllers
{
    public class RoomController : Controller
    {
        private readonly HotelDbContext _context;

        public RoomController(HotelDbContext context) 
        {
            _context = context;
        }

        public async Task<IActionResult> Index() 
        {
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        public IActionResult Create() 
        {
            ViewBag.Rooms = new SelectList(_context.Rooms.ToList(), "RoomId", "RoomNumber");
            return View(); // Shows the form to fill out a new Room
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Checks the hidden token!
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Checks the hidden token!
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.RoomId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Go back to room list
            }
            return View(room);  // If errors, redisplay the form
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Available()
        {
            var availableRooms = await _context.Rooms
                .Where(r => r.IsAvailable)
                .ToListAsync();

            return View(availableRooms);
        }
    }
}
