using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warbler.Repositories;
using Warbler.Services;
using Warbler.Misc;

namespace Warbler.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private UserService UserService { get; }

        public ChatController(WarblerDbContext context)
        {
            UserService = new UserService(new SqlUserRepository(context));
        }

        public async Task<IActionResult> Index()
        {
            var user = await UserService.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account");

            if (await UserService.IsNewAsync(user))
            {
                return RedirectToAction(nameof(Nearby));
            }

            ViewData["Component"] = ViewData["Title"] = "Chatroom";
            ViewData["Fluid"] = true;
            
            return View("ReactComponent", ViewData);
        }

        public IActionResult Nearby()
        {
            ViewData["Title"] = "Select a university";
            ViewData["Heading"] = true;
            ViewData["Component"] = "Proximity"; // Components/Proximity

            return View("ReactComponent", ViewData);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
