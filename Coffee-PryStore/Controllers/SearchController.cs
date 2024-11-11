using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Coffee_PryStore.Models;
    using System.Linq;
    using System.Globalization;

    public class SearchController(DataBaseHome context) : Controller
    {
        private readonly DataBaseHome _context = context;

        [HttpGet]
        public IActionResult Index(string searchTerm)
        {
           
            if (string.IsNullOrEmpty(searchTerm))
            {
               
                return RedirectToAction("Index", "Home"); 
            }

            var products = _context.Table
                .Where(p => p.CofName.Contains(searchTerm))
                .ToList();
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;

            return View("Search", products);
        }

        public IActionResult ChangeLanguage(string culture)
        {

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            Response.Cookies.Append("lang", culture, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1) });

            HttpContext.Session.SetString("Culture", culture);

            return RedirectToAction("PersonRegistration");
        }

    }

}
