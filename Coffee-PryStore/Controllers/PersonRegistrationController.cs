using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Coffee_PryStore.Models.Configurations;

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

        [HttpGet]
        public IActionResult PersonRegistration()
        {
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
            // Хардкодовані дані адміністратора
            string adminEmail = "pryimak@gmail.com";
            string adminPassword = HashPassword("12345678");
            string adminRole = "Admin";

            // Якщо введені дані відповідають хардкодованим даним адміністратора
            if (email == adminEmail && VerifyPassword(adminPassword, password))
            {
                // Призначаємо роль і ID адміністратора в сесії
                HttpContext.Session.SetString("UserRole", adminRole);
                HttpContext.Session.SetInt32("UserId", 0); // ID адміністратора може бути 0 або інше значення
                return RedirectToAction("AdminDashboard", "Admin");
            }

            // Перевірка користувачів у базі даних
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

            // Якщо користувача з такою електронною адресою не існує, створюємо нового користувача
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