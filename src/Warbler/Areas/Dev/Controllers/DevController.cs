using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Warbler.Areas.Chat.Services;
using Warbler.Identity.Data;

namespace Warbler.Areas.Dev.Controllers
{
    [Area("Dev")]
    [Authorize]
    public class DevController : Controller
    {
        private UniversityService UniversityService { get; }

        public DevController(WarblerDbContext context)
        {
            UniversityService = new UniversityService(context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public async Task<string> GetEverything()
        {
            return JsonConvert
                .SerializeObject(await UniversityService.GetUniversities(), Formatting.Indented);
        }
    }
}
