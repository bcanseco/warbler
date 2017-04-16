using Microsoft.AspNetCore.Mvc;
using Warbler.Areas.Chat.Data;

namespace Warbler.Areas.Chat.Controllers
{
    [Area("Chat")]
    public class ChatController : Controller
    {
        private ChatContext Context { get; }

        public ChatController(ChatContext context)
        {
            Context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
