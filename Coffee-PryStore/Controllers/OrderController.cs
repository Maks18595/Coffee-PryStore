using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
