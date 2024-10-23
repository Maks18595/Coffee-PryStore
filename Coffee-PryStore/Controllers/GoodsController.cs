using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class GoodsController : Controller
    {
        public IActionResult Goods()
        {
            return View();
        }
    }
}
