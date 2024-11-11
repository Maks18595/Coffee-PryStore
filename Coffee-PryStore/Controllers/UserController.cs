using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Documents;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Coffee_PryStore.Controllers
{
    public class UserController(DataBaseHome context) : Controller
    {
        private readonly DataBaseHome _context = context;

        public IActionResult UserProfile(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

            var user = _context.Users
                .Include(u => u.Orders) 
                .ThenInclude(o => o.OrderItems) 
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }

        public IActionResult ChangeLanguage(string culture)
        {

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            Response.Cookies.Append("lang", culture, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) });

            HttpContext.Session.SetString("Culture", culture);
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return RedirectToAction("PersonRegistration");
        }


        public IActionResult UserDashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["CurrentUserId"] = user.Id;
            ViewData["UserRole"] = user.Role;

            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Table)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(order);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return RedirectToAction(nameof(UserDashboard));
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Models.User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Email = user.Email;

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    existingUser.Password = HashPassword(user.Password); 
                }

                existingUser.Role = user.Role;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(User));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                }
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }

        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string storedHash, string password)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == storedHash;
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }
    }
}
