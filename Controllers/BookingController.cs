using Microsoft.AspNetCore.Mvc;
using HospitalityProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalityProject.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelDbContext _context;

        public BookingController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings.Include(b => b.Room).ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            var rooms = _context.Rooms.ToList();
            ViewBag.Rooms = rooms;

            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            foreach (var modelState in ModelState)
            {
                var field = modelState.Key;
                var errors = modelState.Value.Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {field}, Error: {error.ErrorMessage}");
                }
            }

            // If validation fails, reload rooms for the select list
            ViewBag.Rooms = _context.Rooms.ToList();

            return View(booking);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            ViewBag.Rooms = new SelectList(_context.Rooms, "RoomId", "RoomNumber", booking.RoomId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.Rooms = new SelectList(_context.Rooms, "RoomId", "RoomNumber", booking.RoomId);
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
