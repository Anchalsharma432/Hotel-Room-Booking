using Microsoft.AspNetCore.Mvc;

namespace HospitalityProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/About
        public ActionResult About()
        {
            ViewBag.Title = "About Us";
            return View();
        }

        // GET: /resort
        [Route("resort")]
        public ActionResult Resort()
        {
            ViewBag.Title = "Our Resort";
            return View();
        }


        // GET: /rooms
        [Route("rooms")]
        public ActionResult Rooms()
        {
            ViewBag.Title = "Our Rooms";
            return View();
        }

        // GET: /rooms/bunk
        [Route("rooms/bunk")]
        public ActionResult BunkRooms()
        {
            ViewBag.Title = "Our Bunk Rooms";
            return View();
        }

        // GET: /rooms/guest
        [Route("rooms/guest")]
        public ActionResult GuestRooms()
        {
            ViewBag.Title = "Our Guest Rooms";
            return View();
        }

        // GET: /beachfrontpool
        [Route("beachfrontpool")]
        public ActionResult BeachFrontPool()
        {
            ViewBag.Title = "BeachFront Pool";
            return View();
        }


        // GET: /beachfrontpool
        [Route("meetings")]
        public ActionResult Meetings()
        {
            ViewBag.Title = "Meeting Room";
            return View();
        }

        // GET: /beachfrontpool
        [Route("dinning")]
        public ActionResult Dinning()
        {
            ViewBag.Title = "Dinning";
            return View();
        }

        // GET: /beachfrontpool
        [Route("spa")]
        public ActionResult Spa()
        {
            ViewBag.Title = "Resort Spa";
            return View();
        }

        public IActionResult DownloadSpaMenu()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/2025-Spa-Menu.pdf");
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "2025-Spa-Menu.pdf");
        }

        // GET: /fitness
        [Route("fitness")]
        public ActionResult Fitness()
        {
            ViewBag.Title = "Fitness";
            return View();
        }

    }
}
