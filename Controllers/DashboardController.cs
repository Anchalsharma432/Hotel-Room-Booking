using HospitalityProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalityProject.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HotelDbContext _context;

        public DashboardController(HotelDbContext context)
        {
            _context = context; 
        }

        public ActionResult Index()
        {
            var totalRooms = _context.Rooms.Count();
            var availableRooms = _context.Rooms.Count(r => r.IsAvailable == true);
            var unavailableRooms = _context.Rooms.Count(r => r.IsAvailable == false);
            var totalBookings = _context.Bookings.Count();
            var totalusers = _context.Users.Count();

            var model = new Dashboard
            {
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                UnavailableRooms = unavailableRooms,
                TotalBookings = totalBookings,
                TotalUsers = totalusers
            };

            return View(model);
        }
    }
}
