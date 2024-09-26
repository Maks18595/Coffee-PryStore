using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class PersonRegistrationController : Controller
    {
        public IActionResult PersonRegistration()
        {
            return View();
        }
    }
}
