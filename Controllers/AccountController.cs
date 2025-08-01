using HospitalityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace HospitalityProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelDbContext _context;

        public AccountController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Login Page
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(model);
        }

        // ✅ Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(user);
                }

                // Validate password match
                if (user.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("Password", "Passwords do not match.");
                    return View(user);
                }

                // Hash password before saving
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // ✅ Profile Page
        public IActionResult Profile()
        {
            // Get username from session
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch user from database
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }


        // ✅ Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
