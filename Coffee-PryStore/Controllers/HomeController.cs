using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Coffee_PryStore.Controllers
{

    public class HomeController(DataBaseHome context) : Controller
    {
        private readonly DataBaseHome _context = context;

        public IActionResult Search()
        {
            return View();
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
                    using var memoryStream = new MemoryStream();
                    await imageFile.CopyToAsync(memoryStream);
                    existingProduct.ImageData = memoryStream.ToArray();
                }

                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Table));
            }
            return View(product);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchName, string searchCategory, string sortOrder)
        {
            var products = from p in _context.Table select p;

            if (!String.IsNullOrEmpty(searchName))
            {
                products = products.Where(p => p.CofName.Contains(searchName));
            }

            if (!String.IsNullOrEmpty(searchCategory))
            {
                products = products.Where(p => p.CofCateg == searchCategory);
            }


            products = sortOrder switch
            {
                "price_asc" => products.OrderBy(p => p.CofPrice),
                "price_desc" => products.OrderByDescending(p => p.CofPrice),
                "name_asc" => products.OrderBy(p => p.CofName),
                "name_desc" => products.OrderByDescending(p => p.CofName),
                _ => products.OrderBy(p => p.CofName),
            };
            var productList = await products.ToListAsync();

            
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

            return View(productList);
        }
        

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Retrieve the product
            var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == productId);
            if (product == null)
            {
                return NotFound();
            }

            // Retrieve cart from session or create a new cart object
            var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart") ?? new Baskets();

            // Get the user ID, ensuring it is not null
            var userId = HttpContext.Session.GetInt32("UserId");

            // Retrieve an existing cart item for this product if it exists in the database
            Basket existingCartItem = null;
            if (userId.HasValue)
            {
                existingCartItem = await _context.Basket.FirstOrDefaultAsync(b => b.CofId == productId && b.Id == userId.Value);
            }

            if (existingCartItem != null)
            {
                // If the item exists in both session and database, update quantity
                existingCartItem.Quantity += quantity;
                var sessionCartItem = cart.Items.FirstOrDefault(b => b.CofId == productId);
                if (sessionCartItem != null)
                {
                    sessionCartItem.Quantity += quantity;
                }
            }
            else
            {
                // Add the new item to both the session cart and database
                var newCartItem = new Basket
                {
                    CofId = productId,
                    Quantity = quantity,
                    Id = userId ?? 0 // Assign a default value if userId is null
                };

                cart.Items.Add(newCartItem);
                await _context.Basket.AddAsync(newCartItem); // Save new item to the database
            }

            // Save updated cart to the session and database
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Basket");
        }


        public async Task<IActionResult> Basket()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Перенаправлення на логін, якщо користувач не авторизований
            }

            // Завантаження корзини для поточного користувача з бази даних
            var cartItems = await _context.Basket
                .Include(b => b.Cof)
                .Where(b => b.Id == userId.Value)
                .ToListAsync();

            // Створення моделі для представлення
            var cart = new Baskets
            {
                Items = cartItems
            };

            // Оновлення сесії корзини
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return View(cart); // Передача корзини в представлення
        }


        private Baskets GetCartFromSession()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart");
            return cart ?? new Baskets();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Find the item in the database
            var cartItem = userId != null
                ? await _context.Basket.FirstOrDefaultAsync(b => b.CofId == productId && b.Id == userId.Value)
                : null;

            if (cartItem != null)
            {
                _context.Basket.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Update session cart
                var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart") ?? new Baskets();
                cart.Items.RemoveAll(i => i.CofId == productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Basket");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart");

            if (cart != null && cart.Items.Any())
            {
                var order = new Order
                {
                    UserId = userId.Value,
                    OrderItems = cart.Items.Select(item => new OrderItem
                    {
                        CofId = item.CofId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Cof.CofPrice
                    }).ToList(),
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Items.Sum(item => item.Quantity * item.Cof.CofPrice),
                    Status = "Pending"
                };

                await _context.Orders.AddAsync(order);
                _context.Basket.RemoveRange(cart.Items); // Видалення елементів корзини з бази даних
                HttpContext.Session.Remove("Cart"); // Очищення сесії корзини
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OrderConfirmation");
        }







        private Table? GetProductById(int cofId)
        {
            return _context.Table.Find(cofId);
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
