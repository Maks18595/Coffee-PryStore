using Microsoft.AspNetCore.Mvc;
using Coffee_PryStore.Models;
using System.Linq;

namespace Coffee_PryStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() 
        { 
            return View();
        }
    }

    public class HomeDataController : Controller
    {
        

        private readonly DataBaseHome _context;

        public HomeDataController(DataBaseHome context)
        {
            _context = context;
        }

      


        // GET: HomeData
        public IActionResult HomeTestIndex()
        {
            var data = _context.HomeDataModels.ToList();
            return View(data);
        }

        // GET: HomeData/Details/5
        public IActionResult Details(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // GET: HomeData/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomeData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HomeDataModel model)
        {
            if (ModelState.IsValid)
            {
                _context.HomeDataModels.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(HomeTestIndex));

            }
            return View(model);
        }

        // GET: HomeData/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: HomeData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, HomeDataModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(HomeTestIndex));

            }
            return View(model);
        }

        // GET: HomeData/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: HomeData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.HomeDataModels.Find(id);
            if (item != null)
            {
                _context.HomeDataModels.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(HomeTestIndex));

        }
    }
}
