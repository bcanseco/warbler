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

        public ChatController(ChatContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // TODO: Debug-only, remove
            var firstServer = await _context.Servers
                .Include(s => s.University)
                .Include(s => s.Channels)
                    .ThenInclude(c => c.Memberships)
                .Include(s => s.Channels)
                    .ThenInclude(c => c.Messages)
                .FirstAsync();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
