using Microsoft.AspNetCore.Mvc;
using HospitalityProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Net.Mail;
using System.Net;

namespace HospitalityProject.Controllers
{
    public class BookingController : Controller
    {
        private readonly HotelDbContext _context;
        private readonly SmtpSettings _smtpSettings;

        public BookingController(HotelDbContext context, IConfiguration configuration)
        {
            _context = context;
            _smtpSettings = configuration.GetSection("Smtp").Get<SmtpSettings>();
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings.Include(b => b.Room).ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            var vm = new BookingViewModel
            {
                // Set default values if needed
                NumberOfGuests = 1
            };
            PopulateBookingViewModel(vm); // Use the helper method
            return View(vm);

        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            // This gives the model binder the lists it needs to validate against.
            PopulateBookingViewModel(model);

            // Add a server-side check for date logic
            if (model.CheckInDate.HasValue && model.CheckOutDate.HasValue && model.CheckOutDate.Value <= model.CheckInDate.Value)
            {
                ModelState.AddModelError(nameof(model.CheckOutDate), "Check-out date must be after the check-in date.");
            }

            // Manual validation for dropdowns, similar to your original code
            if (string.IsNullOrEmpty(model.RoomType))
            {
                ModelState.AddModelError(nameof(model.RoomType), "Please select a room type.");
            }
            if (string.IsNullOrEmpty(model.BedType))
            {
                ModelState.AddModelError(nameof(model.BedType), "Please select a bed type.");
            }

            if (ModelState.IsValid)
            {
                // Step 1: Find all rooms that match the user's TYPE and BED preference.
                var potentialRooms = await _context.Rooms
                    .Where(r => r.RoomType == model.RoomType && r.BedType == model.BedType && r.IsAvailable)
                    .ToListAsync();

                if (!potentialRooms.Any())
                {
                    ModelState.AddModelError("", "Sorry, no rooms are available with the selected type and bed combination.");
                    return View(model);
                }

                // Step 2: Find the FIRST of these rooms that is NOT already booked for the selected dates.
                Room availableRoom = null;
                foreach (var room in potentialRooms)
                {
                    // Check for any existing booking for THIS room that overlaps with the requested dates.
                    bool isBooked = await _context.Bookings
                        .AnyAsync(b => b.RoomId == room.RoomId &&
                                       b.CheckInDate < model.CheckOutDate.Value &&
                                       b.CheckOutDate > model.CheckInDate.Value);

                    if (!isBooked)
                    {
                        availableRoom = room; // We found an available room!
                        break; // Exit the loop
                    }
                }

                // Step 3: If after checking all potential rooms, none are free, return an error.
                if (availableRoom == null)
                {
                    ModelState.AddModelError("", "Sorry, all rooms of that type are booked for the selected dates. Please try other dates.");
                    return View(model);
                }

                // Step 4: We have a valid, available room. Proceed to create the booking.
                var nights = (model.CheckOutDate.Value - model.CheckInDate.Value).Days;
                var totalPrice = availableRoom.PricePerNight * nights;

                // Check if guest already exists (using Email as a unique identifier)
                var existingGuest = _context.Guests
                    .FirstOrDefault(g => g.Email == model.Email);

                if (existingGuest == null)
                {
                    // Guest not found → create and add new guest
                    var guest = new Guest
                    {
                        FirstName = model.GuestName,
                        LastName = model.GuestLastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber
                    };

                    _context.Guests.Add(guest);
                }

                var booking = new Booking
                {
                    GuestName = model.GuestName,
                    GuestLastName = model.GuestLastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    RoomId = availableRoom.RoomId, // Assign the confirmed available RoomId
                    NumberOfGuests = model.NumberOfGuests,
                    CheckInDate = model.CheckInDate.Value,
                    CheckOutDate = model.CheckOutDate.Value,
                    PricePerNight = availableRoom.PricePerNight,
                    TotalPrice = totalPrice,
                    Status = "Confirmed",
                    RoomType = model.RoomType,
                    BedType = model.BedType
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var bookingDetails = new BookingSuccessViewModel
                {
                    BookingId = booking.BookingId,
                    GuestName = booking.GuestName,
                    Email = booking.Email,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    RoomType = booking.RoomType,
                    BedType = booking.BedType,
                    TotalPrice = booking.TotalPrice
                };

                // Send confirmation email
                //await SendBookingConfirmationEmail(bookingDetails);

                return View("BookingSuccess", bookingDetails);
            }

            // If we got here, something was invalid. Repopulate data and return the view.
            return View(model);
        }

        public IActionResult BookingSuccess()
        {
            return View(nameof(BookingSuccess));
        }

      
public async Task SendBookingConfirmationEmail(BookingSuccessViewModel model)
    {
        var mail = new MailMessage();
        mail.From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            // Log or throw a meaningful error or just skip sending email
            throw new ArgumentException("Guest email address is missing.");
        }

        mail.To.Add(model.Email);
        mail.Subject = "Booking Confirmation";

        var body = $@"
            <h2>Booking Confirmation</h2>
            <p>Dear {model.GuestName} {model.GuestLastName},</p>
            <p>Thank you for your booking! Here are your booking details:</p>
            <ul>
                <li><strong>Booking ID:</strong> {model.BookingId}</li>
                <li><strong>Room Type:</strong> {model.RoomType}</li>
                <li><strong>Bed Type:</strong> {model.BedType}</li>
                <li><strong>Number of Guests:</strong> {model.NumberOfGuests}</li>
                <li><strong>Check-In Date:</strong> {model.CheckInDate:yyyy-MM-dd}</li>
                <li><strong>Check-Out Date:</strong> {model.CheckOutDate:yyyy-MM-dd}</li>
                <li><strong>Price Per Night:</strong> ${model.PricePerNight:F2}</li>
                <li><strong>Total Price:</strong> ${model.TotalPrice:F2}</li>
                <li><strong>Status:</strong> {model.Status}</li>
            </ul>
            <p>We look forward to welcoming you!</p>
            <p>Best regards,<br/>Your Hotel Team</p>";
        
        mail.Body = body;
        mail.IsBodyHtml = true;

        using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
        smtp.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
        smtp.EnableSsl = _smtpSettings.EnableSsl;

        Console.WriteLine($"Sending email to: {model.Email}");

        await smtp.SendMailAsync(mail);
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

        // --- Private Helper Method to load dropdown data ---
        private void PopulateBookingViewModel(BookingViewModel vm)
        {
            var roomTypes = _context.Rooms
                .Where(r => !string.IsNullOrEmpty(r.RoomType))
                .Select(r => r.RoomType)
                .Distinct()
                .OrderBy(rt => rt)
                .ToList();

            var roomData = roomTypes.Select(rt => new
            {
                RoomType = rt,
                BedTypes = _context.Rooms
                    .Where(r => r.RoomType == rt && !string.IsNullOrEmpty(r.BedType))
                    .GroupBy(r => r.BedType)
                    .Select(g => new
                    {
                        BedType = g.Key,
                        PricePerNight = g.Select(x => x.PricePerNight).FirstOrDefault()
                    })
                    .OrderBy(x => x.BedType)
                    .ToList()
            }).ToList();

            // Pass this data as a JSON string to the view for JavaScript
            ViewBag.RoomDataJson = JsonSerializer.Serialize(roomData);

            vm.Rooms = roomTypes.Select(rt => new SelectListItem(rt, rt));

            // If a RoomType is already selected (e.g., on POST validation failure), populate the BedTypes
            if (!string.IsNullOrEmpty(vm.RoomType))
            {
                var selectedRoomData = roomData.FirstOrDefault(r => r.RoomType == vm.RoomType);
                if (selectedRoomData != null)
                {
                    vm.BedTypes = selectedRoomData.BedTypes.Select(b => new SelectListItem(b.BedType, b.BedType));
                }
            }
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
