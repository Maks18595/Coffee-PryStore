using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Coffee_PryStore.Controllers
{
    public class UserController : Controller
    {
        private readonly DataBaseHome _context;

        public UserController(DataBaseHome context)
        {
            _context = context;
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

            ViewData["CurrentUserId"] = user.Id; // Зберігаємо ID користувача у ViewData
            return View(user);
        }


        // Видалення користувача
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
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
            return RedirectToAction(nameof(UserDashboard));
        }

        // Редагування користувача (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Редагування користувача (POST)
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Email = user.Email;

                // Змінюємо пароль тільки якщо він не пустий
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    existingUser.Password = user.Password;
                }

                existingUser.Role = user.Role;

                try
                {
                    _context.SaveChanges();
                    return RedirectToAction(nameof(UserDashboard));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                }
            }
            return View(user);
        }

        // Деталі користувача
        [HttpGet]
        public IActionResult Details(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
    }
}
