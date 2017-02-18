using Microsoft.AspNetCore.Mvc;
using Warbler.Infrastructure.Chat.Services;

namespace Warbler.Areas.Chat.Controllers
{
    [Area("Chat")]
    public class SandboxController : Controller
    {
        private SandboxService Service { get; set; }

        private SandboxController(SandboxService sandboxService)
        {
            Service = sandboxService;
        }

        public SandboxController() 
            : this(new SandboxService())
        { }

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
