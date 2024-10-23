using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Coffee_PryStore.Controllers
{
    public class PersonRegistrationController(DataBaseHome context) : Controller
    {
        private readonly DataBaseHome _context = context;

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

        [HttpPost]
        public IActionResult PersonRegistration(string email, string password, string role)
        {
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Будь ласка, введіть електронну адресу та пароль.");
                return View();
            }

       
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
              
                if (VerifyPassword(existingUser.Password, password))
                {
                   
                    HttpContext.Session.SetInt32("UserId", existingUser.Id);

                    
                    return existingUser.Role == "Admin"
                        ? RedirectToAction("AdminDashboard", "Admin")
                        : RedirectToAction("UserDashboard", "User", new { id = existingUser.Id });
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
                Role = role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newUser.Id); 

            return newUser.Role == "Admin"
                ? RedirectToAction("AdminDashboard", "Admin")
                : RedirectToAction("UserDashboard", "User", new { id = newUser.Id });
        }


       

       
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

     
        private bool VerifyPassword(string storedHash, string password)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == storedHash;
        }
    }
}