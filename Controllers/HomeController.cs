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
    }
}
