using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Coffee_PryStore.Models;
    using System.Linq;

    public class SearchController : Controller
    {
        private readonly DataBaseHome _context;

        public SearchController(DataBaseHome context)
        {
            _context = context;
        }

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

         
            return View("Search", products);
        }
    }

}
