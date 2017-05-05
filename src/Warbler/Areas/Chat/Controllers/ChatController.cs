using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Warbler.Areas.Chat.Controllers
{
    [Area("Chat")]
    [Authorize]
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Chatroom()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
