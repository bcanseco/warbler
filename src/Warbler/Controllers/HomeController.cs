﻿using Microsoft.AspNetCore.Mvc;

namespace Warbler.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Documents()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
