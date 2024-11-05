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
            // Отримуємо ID користувача з сесії
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Перенаправляємо на сторінку реєстрації, якщо ID користувача відсутній у сесії
                return RedirectToAction("PersonRegistration", "PersonRegistration");
            }

            // Отримуємо товари з кошика, що відповідають користувачу
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
                // Передаємо елементи корзини у випадку помилки
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

            // Отримуємо товари з кошика
            var cartItems = await _context.Basket
                .Include(b => b.Cof)
                .Where(b => b.Id == userId.Value)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Ваш кошик порожній.";
                return RedirectToAction("Basket");
            }

            // Перевіряємо, чи достатньо товару на складі для кожного елемента
            foreach (var item in cartItems)
            {
                var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == item.CofId);
                if (product != null && product.CofAmount < item.Quantity)
                {
                    TempData["ErrorMessage"] = $"Товару '{product.CofName}' недостатньо для замовлення.";
                    return RedirectToAction("Basket");
                }
            }

            // Обчислюємо загальну суму
            var totalAmount = cartItems.Sum(item => item.Quantity * item.Cof.CofPrice);

            // Створюємо об'єкт замовлення
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

            // Додаємо замовлення до бази даних
            _context.Orders.Add(order);

            // Зменшуємо кількість товару в базі даних
            foreach (var item in cartItems)
            {
                var product = await _context.Table.FirstOrDefaultAsync(p => p.CofId == item.CofId);
                if (product != null)
                {
                    product.CofAmount -= item.Quantity;
                    _context.Table.Update(product);
                }
            }

            // Видаляємо товари з кошика після оформлення замовлення
            _context.Basket.RemoveRange(cartItems);

            // Очищуємо сесію кошика, якщо потрібно
            HttpContext.Session.Remove("Cart");

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Замовлення успішно оформлено.";
            return RedirectToAction("OrderSuccess");
        }

    }
}
