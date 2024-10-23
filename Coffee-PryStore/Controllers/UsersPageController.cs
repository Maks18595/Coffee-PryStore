using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace Coffee_PryStore.Controllers
{
    public class UsersPageController : Controller
    {
        private readonly DataBaseHome _context;

        public UsersPageController(DataBaseHome context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(new User());
        }



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
            return View(user);
        }


        public IActionResult Users()
        {
            var users = _context.Users.ToList(); 
            return View(users); 
        }

    
    }
}