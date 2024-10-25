using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Coffee_PryStore.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
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
        [Authorize]
        public async Task<IActionResult> AddToCart(int cofId, int quantity = 1)
        {
            var product = await _context.Table.FindAsync(cofId);
            if (product == null)
            {
                return NotFound(); 
            }

            var cart = GetCartFromSession(); 

            var cartItem = cart.Items.FirstOrDefault(ci => ci.CofId == cofId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity; 
            }
            else
            {
                cart.Items.Add(new Basket { CofId = cofId, Quantity = quantity, Cof = product });
            }

            SaveCartToSession(cart); 

            return RedirectToAction("Index", "Home");
        }

       
        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartFromSession(); 
            return View(cart); 
        }

        [Authorize]
        public IActionResult Basket()
        {
            var cart = GetCartFromSession(); 
            return View(cart); 
        }

        private Baskets GetCartFromSession()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Baskets>("Cart");
            return cart ?? new Baskets();
        }

        private void SaveCartToSession(Baskets cart)
        {
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        public decimal CalculateTotalSum(Baskets cart)
        {
            return cart.Items.Sum(item => item.Cof.CofPrice * item.Quantity);
        }
    }
}
