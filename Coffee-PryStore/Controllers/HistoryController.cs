﻿using Microsoft.AspNetCore.Mvc;

namespace Coffee_PryStore.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult History()
        {
            var currentLanguage = Request.Cookies["lang"] ?? "en-US"; 
            ViewData["CurrentLanguage"] = currentLanguage;
            return View();
        }
    }
}
