using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Coffee_PryStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataBaseHome _context;

        public HomeController(DataBaseHome context)
        {
            _context = context;
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


        [AllowAnonymous]
        public IActionResult Index()
        {
            var products = _context.Table.ToList(); // Отримання списку продуктів з бази даних

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                if (user != null)
                {
                    ViewBag.UserEmail = user.Email;
                    ViewBag.UserRole = user.Role;
                }
            }
            return View(products); // Переконайтеся, що модель передається правильно
        }





    }



    public class HomeDataController : Controller
    {
        private readonly DataBaseHome _context;

        public HomeDataController(DataBaseHome context)
        {
            _context = context;
        }

        // GET: HomeData
        public IActionResult HomeTestIndex()
        {
            var data = _context.HomeDataModels.ToList();
            return View(data);
        }

        // GET: HomeData/Details/5
        public IActionResult Details(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: HomeData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomeData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HomeDataModel model)
        {
            if (ModelState.IsValid)
            {
                _context.HomeDataModels.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(HomeTestIndex));
            }
            return View(model);
        }

        // GET: HomeData/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: HomeData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, HomeDataModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(HomeTestIndex));
            }
            return View(model);
        }

        // GET: HomeData/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: HomeData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item != null)
            {
                _context.HomeDataModels.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(HomeTestIndex));
        }
    }
}
