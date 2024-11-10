using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;



namespace Coffee_PryStore.Controllers
{
    public class AdminController(DataBaseHome context) : Controller
    {
        private readonly DataBaseHome _context = context;

        public async Task<IActionResult> AdminDashboard()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null || users.Count == 0)
            {
                return View("NoUsers");
            }

            var currentAdmin = users.FirstOrDefault(u => u.Role == "Admin");
            if (currentAdmin != null)
            {
                ViewData["CurrentUserId"] = currentAdmin.Id;
                ViewData["UserRole"] = currentAdmin.Role; 
            }

            return View(users);
        }



        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Table) 
                .ToListAsync();
            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Table)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
       

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetails", new { id });
        }

     
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); 
        }




        [HttpPost]
        public IActionResult UpdateUserPassword(int userId, string currentPassword, string newPassword)
        {
    
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                ModelState.AddModelError("", "Користувача не знайдено.");
                return View();
            }

         
            if (!VerifyPassword(user.Password, currentPassword))
            {
                ModelState.AddModelError("", "Поточний пароль невірний.");
                return View();
            }

          
            user.Password = HashPassword(newPassword);

          
            _context.Users.Update(user);
            _context.SaveChanges();

            TempData["Message"] = "Пароль успішно змінено.";
            return RedirectToAction("AdminDashboard", "Admin");
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
            var users = await _context.Users.ToListAsync(); 
            return View(users); 
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
                user.Password = HashPassword(user.Password); 
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
            if (users == null || users.Count == 0)
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
                    existingUser.Password = HashPassword(user.Password); 
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



        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Table product, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await ImageFile.CopyToAsync(memoryStream);
                    product.ImageData = memoryStream.ToArray();
                }

                await _context.AddAsync(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProductDetails", "Admin", new { id = product.CofId });
            }

            return View(product);
        }





        public IActionResult ProductDetails()
        {
            var products = _context.Table.ToList(); 
            return View(products); 
        }




        public IActionResult EditProduct(int id)
        {
            var product = _context.Table.FirstOrDefault(p => p.CofId == id);
            if (product == null)
            {
                return NotFound();
            }

       
            if (product.ImageData != null && product.ImageData.Length > 0)
            {
                ViewBag.ExistingImage = Convert.ToBase64String(product.ImageData);
            }

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> EditProduct(Table model, string ImageChoice, IFormFile ImageFile)
        {
            var product = await _context.Table.FindAsync(model.CofId);

            if (product == null)
            {
                return NotFound();
            }

            product.CofName = model.CofName;
            product.CofCateg = model.CofCateg;
            product.CofPrice = model.CofPrice;
            product.CofAmount = model.CofAmount;
            product.CofDuration = model.CofDuration;
            product.Description = model.Description; 

            if (ImageChoice == "new" && ImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(memoryStream);
                    product.ImageData = memoryStream.ToArray(); 
                }
            }

            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductDetails", new { id = product.CofId });
        }




        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Table.FirstOrDefault(p => p.CofId == id);
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
            else
            {
                return NotFound();
            }

            return RedirectToAction("ProductDetails");
        }




    }
}
