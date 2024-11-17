using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class DeliveryPayController : Controller
    {
        public IActionResult DeliveryPay()
        {
            var currentLanguage = Request.Cookies["lang"] ?? "en-US";
            ViewData["CurrentLanguage"] = currentLanguage;
            return View();
        }
    }
}
