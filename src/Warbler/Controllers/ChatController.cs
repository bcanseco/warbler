using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
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
        private UniversityService UniversityService { get; }
        private AuthConfigService AuthConfigService { get; }
        private ChannelTemplateService ChannelTemplateService { get; }

        public ChatController(WarblerDbContext context)
        {
            UserService = new UserService(new SqlUserRepository(context));
            UniversityService = new UniversityService(new SqlUniversityRepository(context));
            AuthConfigService = new AuthConfigService(new SqlAuthConfigRepository(context), null);
            ChannelTemplateService = new ChannelTemplateService(new SqlChannelTemplateRepository(context));
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

        [HttpPost]
        public async Task<string> Join([FromBody] NearByResult userChoice)
        {
            var user = await UserService.FindByNameAsync(User.Identity.Name);

            var university = await UniversityService.GetOrCreateAsync(userChoice, await ChannelTemplateService.GetAsync());

            var isAlreadyMember = university.Server.Channels
                .Any(ch => ch.Memberships.Any(m => m.User.Equals(user)));

            if (isAlreadyMember)
            {
                return "/Chat";
            }
            if (university.Server.IsAuthEnabled)
            {
                var partner = await AuthConfigService.GetConfigAsync(university);
                return $"/Account/SingleSignOn?partnerName={partner.Name}&relayState={user.Id},{university.Id}";
            }
            await UniversityService.JoinAsync(user, university);
            return "/Chat";
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
