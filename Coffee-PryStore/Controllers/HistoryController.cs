using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult History()
        {
            return View();
        }
    }
}
