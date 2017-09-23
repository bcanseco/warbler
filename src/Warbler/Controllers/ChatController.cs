using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warbler.Models;
using Warbler.Repositories;
using Warbler.Services;
using Warbler.Misc;

namespace Warbler.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private UserManager<User> UserManager { get; }
        private UserService UserService { get; }

        public ChatController(UserManager<User> userManager, WarblerDbContext context)
        {
            UserManager = userManager;
            UserService = new UserService(new SqlUserRepository(context));
        }

        public async Task<IActionResult> Index()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (await UserService.IsNewAsync(user))
            {
                ViewData["Title"] = "Select a university";
                ViewData["Heading"] = true;
                ViewData["Component"] = "Proximity"; // Components/Proximity
            }
            else
            {
                ViewData["Component"] = ViewData["Title"] = "Chatroom";
                ViewData["Fluid"] = true;
            }
            
            return View("ReactComponent", ViewData);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
