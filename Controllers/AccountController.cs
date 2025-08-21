using HospitalityProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // ✅ Create claims for authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role) // Role-based access
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // Remember user
                    };

                    // ✅ Sign in user with cookie
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

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
        public async Task<IActionResult> Register(User user, string ConfirmPassword)
        {
            // Show validation errors
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation Error: " + error.ErrorMessage);
                }
                return View(user);
            }

            // Username uniqueness check
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(user);
            }

            // Password match check
            if (user.Password != ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Passwords do not match.");
                return View(user);
            }

            // Password length check
            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
                return View(user);
            }

            // Assign default role
            user.Role = "User";

            // Hash password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Registration successful! Please login.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error: " + ex.Message);
                ModelState.AddModelError("", "An error occurred while saving your data.");
                return View(user);
            }

            return RedirectToAction("Login");
        }



        // ✅ Show Profile Page (GET)
        [HttpGet]
        public IActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User model, string newPassword)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            var username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login");

            // Update fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Phone = model.Phone;

            if (!string.IsNullOrEmpty(newPassword))
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.SaveChanges();

            ViewBag.Message = "Profile updated successfully!";
            return View(user);
        }


        // ✅ Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
