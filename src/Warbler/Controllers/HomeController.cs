using Microsoft.AspNetCore.Mvc;

namespace Warbler.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Slides()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
