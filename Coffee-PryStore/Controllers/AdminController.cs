using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;



namespace Coffee_PryStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataBaseHome _context;

        public AdminController(DataBaseHome context)
        {
            _context = context;
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                return View("NoUsers"); // Якщо немає користувачів
            }

            var currentAdmin = users.FirstOrDefault(u => u.Role == "Admin");
            if (currentAdmin != null)
            {
                ViewData["CurrentUserId"] = currentAdmin.Id;
            }

            return View(users);
        }

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

        [HttpGet]
        public IActionResult EmptyPage()
        {
            return View();
        }

       

        [HttpGet]
        public async Task<IActionResult> TableAsync()
        {
            var products = await _context.Table.ToListAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync(); // Отримання списку всіх користувачів
            return View(users); // Передача списку користувачів до представлення
        }

        [HttpGet]
        public IActionResult Categories()
        {
            return View(new User());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new User());
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminDashboard));
            }
            return View(user);
        }

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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            var users = await _context.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                return View("NoUsers");
            }

            return RedirectToAction(nameof(AdminDashboard));
        }

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

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
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
                    existingUser.Password = user.Password; // Тут ви можете хешувати пароль
                }

                existingUser.Role = user.Role;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Users));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                }
            }
            return View(user);
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View(new Table()); // Передайте новий екземпляр Table
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Table product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/Attributes/Images", imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.ImagePath = "/Attributes/Images/" + imageFile.FileName;
                }

                // Перевірте, чи продукт уже існує в базі даних перед додаванням
                await _context.Table.AddAsync(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Замініть на правильне перенаправлення
            }
            return View(product);
        }


        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Table.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Table product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Table.FindAsync(product.CofId);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.CofName = product.CofName;
                existingProduct.CofCateg = product.CofCateg;
                existingProduct.CofPrice = product.CofPrice;
                existingProduct.CofAmount = product.CofAmount;
                existingProduct.CofDuration = product.CofDuration;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var filePath = Path.Combine("wwwroot/Attributes/Images", imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    existingProduct.ImagePath = "/Attributes/Images/" + imageFile.FileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Table));
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Table.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.Table.FindAsync(id);
            if (product != null)
            {
                _context.Table.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Table));
        }
    }
}
