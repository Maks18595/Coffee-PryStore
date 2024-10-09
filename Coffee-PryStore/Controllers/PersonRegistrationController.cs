using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Security.Cryptography;
using System.Text;

namespace Coffee_PryStore.Controllers
{
    public class PersonRegistrationController : Controller
    {
        private readonly DataBaseHome _context; // Your DbContext for accessing the database

        public PersonRegistrationController(DataBaseHome context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult PersonRegistration()
        {
            return View();
        }

        public IActionResult UserProfile(int id)
        {
            // Перевірка, чи користувач увійшов в систему
            var userId = HttpContext.Session.GetInt32("UserId");

            // Якщо користувач не увійшов, перенаправляємо на сторінку входу
            if (userId == null)
            {
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

            // Шукаємо користувача за параметром id
            var user = _context.Users.Find(id);

            // Якщо користувача не знайдено, повертаємо 404
            if (user == null)
            {
                return NotFound();
            }

            // Відображаємо профіль користувача
            return View(user);
        }


        [HttpPost]
        public IActionResult PersonRegistration(string email, string password, string role)
        {
            // Check if email and password are provided
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Будь ласка, введіть електронну адресу та пароль.");
                return View(); // Return to the registration/login form
            }

            // Check if the user already exists in the database
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                // User exists, validate the password for login
                if (VerifyPassword(existingUser.Password, password)) // Verify the hashed password
                {
                    // Збереження інформації про користувача в сесію після успішного логіну
                    HttpContext.Session.SetInt32("UserId", existingUser.Id);
                    HttpContext.Session.SetString("UserRole", existingUser.Role);

                    // Redirect based on user role
                    if (existingUser.Role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else if (existingUser.Role == "User")
                    {
                        return RedirectToAction("UserDashboard", "User", new { id = existingUser.Id });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Невірний пароль.");
                    return View(); // Return to the form with an error
                }

                ModelState.AddModelError("Email", "Акаунт з такою електронною адресою вже існує. Будь ласка, увійдіть.");
                return View(); // Return to the form with an error message
            }

            // User does not exist, proceed to register a new user
            var newUser = new User
            {
                Email = email,
                Password = HashPassword(password), // Hash the password for security
                Role = role
            };

            // Add the new user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Збереження інформації про нового користувача в сесію після реєстрації
            HttpContext.Session.SetInt32("UserId", newUser.Id);
            HttpContext.Session.SetString("UserRole", newUser.Role);

            // Redirect based on the new user's role
            if (newUser.Role == "Admin")
            {
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else if (newUser.Role == "User")
            {
                return RedirectToAction("UserDashboard", "User", new { id = newUser.Id });
            }

            ModelState.AddModelError("", "Виникла помилка при реєстрації. Спробуйте ще раз.");
            return View(); // If something goes wrong, return to the form
        }

        // Method to hash the password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Method to verify the password
        private bool VerifyPassword(string storedHash, string password)
        {
            var hashedInputPassword = HashPassword(password);
            return hashedInputPassword == storedHash;
        }
    }
}
