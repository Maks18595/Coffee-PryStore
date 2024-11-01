using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Coffee_PryStore.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
   

    public class BasketController : Controller
    {
        private readonly DataBaseHome _context;

        public BasketController(DataBaseHome context)
        {
            _context = context;
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
                _context.Basket.RemoveRange(cart.Items); 
                HttpContext.Session.Remove("Cart"); 
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("OrderConfirmation");
        }


    }








}

