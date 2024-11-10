using Coffee_PryStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Coffee_PryStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataBaseHome _context;

        public OrderController(DataBaseHome context)
        {
            _context = context;
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult Order()
        {
          
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
             
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

     
            var basketItems = (from b in _context.Basket
                               join p in _context.Table on b.CofId equals p.CofId
                               where b.Id == userId.Value
                               select new
                               {
                                   CofId = b.CofId,
                                   CofName = p.CofName,
                                   CofPrice = p.CofPrice,
                                   CofQuantity = b.Quantity
                               }).ToList();

            ViewBag.Products = basketItems;

            return View(new OrderViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Order(OrderViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

            if (!ModelState.IsValid)
            {
            
                var basketItemsView = (from b in _context.Basket
                                       join p in _context.Table on b.CofId equals p.CofId
                                       where b.Id == userId.Value
                                       select new
                                       {
                                           CofId = b.CofId,
                                           CofName = p.CofName,
                                           CofPrice = p.CofPrice,
                                           CofQuantity = b.Quantity
                                       }).ToList();

                ViewBag.Products = basketItemsView;
                return View(model);
            }

        
            var cartItems = await _context.Basket
                .Include(b => b.Cof)
                .Where(b => b.Id == userId.Value)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Ваш кошик порожній.";
                return RedirectToAction("Basket");
            }

            foreach (var item in cartItems)
            {
                var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == item.CofId);
                if (product != null && product.CofAmount < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"Товару '{product.CofName}' недостатньо для замовлення.";
                    return RedirectToAction("Basket");
                }
            }

            var totalAmount = cartItems.Sum(item => item.Quantity * item.Cof.CofPrice);

            var order = new Order
            {
                UserId = userId.Value,
                FullName = model.FullName,
                City = model.City,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                OrderDate = DateTime.Now,
                Status = "New",
                TotalAmount = totalAmount,
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    CofId = item.CofId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Cof.CofPrice
                }).ToList()
            };

            _context.Orders.Add(order);

            foreach (var item in cartItems)
            {
                var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == item.CofId);
                if (product != null)
                {
                    product.CofAmount -= item.Quantity;
                    _context.Table.Update(product);
                }
            }

            _context.Basket.RemoveRange(cartItems);

            HttpContext.Session.Remove("Cart");

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Замовлення успішно оформлено.";
            return RedirectToAction("OrderSuccess");
        }

    }
}
