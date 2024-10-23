using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Basket()
        {
            return View();
        }
    }
}
