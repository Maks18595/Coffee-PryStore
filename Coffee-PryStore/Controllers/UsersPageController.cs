using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;


namespace Coffee_PryStore.Controllers
{
    public class UsersPageController(DataBaseHome context) : Controller
    {
        private readonly DataBaseHome _context = context;


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

            
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    existingUser.Password = user.Password; 
                }

                existingUser.Role = user.Role;

                try
                {
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Users));
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


        public IActionResult Users()
        {
            var users = _context.Users.ToList(); 
            return View(users); 
        }
        public IActionResult ChangeLanguage(string culture)
        {

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

       
            Response.Cookies.Append("lang", culture, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) });

         
            HttpContext.Session.SetString("Culture", culture);

            return RedirectToAction("PersonRegistration");
        }


    }
}