using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Coffee_PryStore.Models.Configurations;
using System.Globalization;

namespace Coffee_PryStore.Controllers
{
    public class PersonRegistrationController : Controller
    {
        private readonly DataBaseHome _context;
        private readonly TokenService _tokenService;

        public PersonRegistrationController(DataBaseHome context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
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



        [HttpGet]
        public IActionResult PersonRegistration()
        {
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View();
        }

        public IActionResult UserProfile(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View(user);
        }

        public IActionResult Logout()
        {
        
            HttpContext.Session.Clear();

       
            return RedirectToAction("PersonRegistration", "PersonRegistration");
        }

       
        [HttpPost]
        public IActionResult PersonRegistration(string email, string password)
        {

            string adminEmail = "pryimak@gmail.com";
            string adminPassword = HashPassword("12345678");
            string adminRole = "Admin";

            if (email == adminEmail && VerifyPassword(adminPassword, password))
            {

                HttpContext.Session.SetString("UserRole", adminRole);
                HttpContext.Session.SetInt32("UserId", 0); 
                return RedirectToAction("AdminDashboard", "Admin");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                if (VerifyPassword(existingUser.Password, password))
                {
                    HttpContext.Session.SetInt32("UserId", existingUser.Id);
                    HttpContext.Session.SetString("UserRole", existingUser.Role);

                    if (existingUser.Role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("UserDashboard", "User", new { id = existingUser.Id });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Невірний пароль.");
                    return View();
                }
            }

            var newUser = new User
            {
                Email = email,
                Password = HashPassword(password),
                Role = "User"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.Id);
            HttpContext.Session.SetString("UserRole", newUser.Role);
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return RedirectToAction("UserDashboard", "User", new { id = newUser.Id });
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
    }
}