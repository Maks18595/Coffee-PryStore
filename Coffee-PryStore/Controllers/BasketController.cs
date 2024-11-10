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
        public async Task<IActionResult> UpdateCartQuantity(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

         
            var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == productId);
            if (product == null || quantity > product.CofAmount)
            {
                TempData["ErrorMessage"] = "Неможливо оновити кількість. Бажана кількість перевищує доступну.";
                return RedirectToAction("Basket");
            }

            var cartItem = await _context.Basket.FirstOrDefaultAsync(b => b.CofId == productId && b.Id == userId.Value);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();

                var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart");
                if (cart != null)
                {
                    var sessionItem = cart.Items.FirstOrDefault(i => i.CofId == productId);
                    if (sessionItem != null)
                    {
                        sessionItem.Quantity = quantity;
                        HttpContext.Session.SetObjectAsJson("Cart", cart);
                    }
                }
            }

            return RedirectToAction("Basket");
        }





        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
    
            var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart") ?? new Baskets();

            var userId = HttpContext.Session.GetInt32("UserId");

            Basket existingCartItem = null;
            if (userId.HasValue)
            {
                existingCartItem = await _context.Basket.FirstOrDefaultAsync(b => b.CofId == productId && b.Id == userId.Value);
            }

            if (existingCartItem != null)
            {

                existingCartItem.Quantity += quantity;
                var sessionCartItem = cart.Items.FirstOrDefault(b => b.CofId == productId);
                if (sessionCartItem != null)
                {
                    sessionCartItem.Quantity += quantity;
                }
            }
            else
            {

                var newCartItem = new Basket
                {
                    CofId = productId,
                    Quantity = quantity,
                    Id = userId ?? 0 
                };

                cart.Items.Add(newCartItem);
                await _context.Basket.AddAsync(newCartItem); 
            }

          
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Basket");
        }

        
     
            public async Task<IActionResult> Basket()
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId == null)
                {
                    return RedirectToAction("PersonRegistration", "PersonRegistration"); 
                }

               
                var cartItems = await _context.Basket
                    .Include(b => b.Cof)
                    .Where(b => b.Id == userId.Value)
                    .ToListAsync();

        
                var cart = new Baskets
                {
                    Items = cartItems
                };

         
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                return View(cart); 
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

        
            var cartItem = userId != null
                ? await _context.Basket.FirstOrDefaultAsync(b => b.CofId == productId && b.Id == userId.Value)
                : null;

            if (cartItem != null)
            {
                _context.Basket.Remove(cartItem);
                await _context.SaveChangesAsync();

                var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart") ?? new Baskets();
                cart.Items.RemoveAll(i => i.CofId == productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Basket");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
           
            return RedirectToAction("Order", "Order");
        }



    }








}

