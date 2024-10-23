using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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
                    using (var memoryStream = new MemoryStream())
                    {
                        await ImageFile.CopyToAsync(memoryStream);
                        product.ImageData = memoryStream.ToArray();
                    }
                }

                await _context.AddAsync(product);
                await _context.SaveChangesAsync(); 
                return RedirectToAction("Index", "Home");
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
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        existingProduct.ImageData = memoryStream.ToArray(); 
                    }
                }

                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Table));
            }
            return View(product);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            var products = await _context.Table.ToListAsync();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user != null)
                {
                    ViewBag.UserEmail = user.Email;
                    ViewBag.UserRole = user.Role;
                }
            }

            return View(products);
        }


        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
          
            var product = _context.Table.Find(productId);

            if (product != null)
            {
              
                var cartItem = _context.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem != null)
                {
                  
                    cartItem.Quantity++;
                }
                else
                {
                    
                    cartItem = new CartItem
                    {
                        ProductId = product.CofId,
                        Quantity = 1 
                    };
                    _context.CartItems.Add(cartItem);
                }

              
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private Table? GetProductById(int productId)
        {
            return _context.Table.Find(productId);
        }




    }


    /*
    public class HomeDataController : Controller
    {
        private readonly DataBaseHome _context;

        public HomeDataController(DataBaseHome context)
        {
            _context = context;
        }

     
        public IActionResult HomeTestIndex()
        {
            var data = _context.HomeDataModels.ToList();
            return View(data);
        }

       
        public IActionResult Details(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

      
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

  
        public IActionResult Edit(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

   
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

    
        public IActionResult Delete(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

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
    */
}
