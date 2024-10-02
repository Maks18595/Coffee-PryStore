using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Coffee_PryStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataBaseHome _context;

        public AdminController(DataBaseHome context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            // Отримати поточного користувача
            var user = User.Identity.Name;
            // Перевірити, чи є користувач адміністратором
            var isAdmin = User.IsInRole("Admin");

            // Для відлагодження вивести інформацію у консоль (або лог)
            Console.WriteLine($"Користувач: {user}, Адміністратор: {isAdmin}");

            if (!isAdmin)
            {
                // Якщо не адміністратор, перенаправити на сторінку доступу заборонено
                return RedirectToAction("AccessDenied", "Home"); // Наприклад, створіть сторінку AccessDenied
            }

            // Ваш код для отримання даних та відображення в AdminDashboard
            var users = _context.Users.ToList(); // Отримати всіх користувачів
            return View(users);
        }
    

    }
}
