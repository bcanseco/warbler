using Microsoft.AspNetCore.Mvc;

namespace Warbler.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Proposal()
        {
            return View();
        }

        public IActionResult Sandbox()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
