using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class PersonRegistrationController : Controller
    {
        [HttpGet]
        public IActionResult PersonRegistration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PersonRegistration(string email, string password, string role)
        {
            // Проста перевірка ролі користувача
            if (role == "Admin")
            {
                // Перенаправляємо до сторінки адміністратора
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else if (role == "User")
            {
                // Перенаправляємо до сторінки користувача
                return RedirectToAction("UserDashboard", "User");
            }

            // Якщо не вибрано роль, повертаємо назад на форму реєстрації
            return View();
        }
       
    }
}