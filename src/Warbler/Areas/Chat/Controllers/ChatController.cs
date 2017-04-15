using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warbler.Areas.Chat.Data;

namespace Warbler.Areas.Chat.Controllers
{
    [Area("Chat")]
    public class ChatController : Controller
    {
        private readonly ChatContext _context;

        private ChatController(ChatContext context)
        {
            _context = context;
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
