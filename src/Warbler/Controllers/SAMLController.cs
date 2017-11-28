using System.Threading.Tasks;
using ComponentSpace.Saml2;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warbler.Misc;
using Warbler.Models;
using Warbler.Repositories;
using Warbler.Services;

namespace Warbler.Controllers
{
    public class SamlController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ISamlServiceProvider _samlServiceProvider;
        private readonly WarblerDbContext _dbContext;

        public SamlController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ISamlServiceProvider samlServiceProvider,
            WarblerDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _samlServiceProvider = samlServiceProvider;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> AssertionConsumerService()
        {
            // Receive and process the SAML assertion contained in the SAML response.
            // The SAML response is received either as part of IdP-initiated or SP-initiated SSO.
            var ssoResult = await _samlServiceProvider.ReceiveSsoAsync();
            
            if (string.IsNullOrEmpty(ssoResult.RelayState))
            {
                // This was a test initiated from the manage page
                return RedirectToAction("Index", "Manage", new { Message = ManageController.ManageMessageId.TestSamlSuccess });
            }

            // This is a mess, but there's no other way to find the current user because
            // this controller hides the current identity for some reason
            var user = await _userManager.FindByIdAsync(ssoResult.RelayState.Split(',')[0]);

            var universityService = new UniversityService(new SqlUniversityRepository(_dbContext));
            var university = await universityService.FindByIdAsync(int.Parse(ssoResult.RelayState.Split(',')[1]));

            var authConfigService = new AuthConfigService(new SqlAuthConfigRepository(_dbContext), null);
            var authConfig = await authConfigService.GetConfigAsync(university);

            await universityService.JoinAsync(user, university, authConfig.OverrideUsernames ? ssoResult.UserID : null);

            return RedirectToAction("Index", "Chat");
        }

        public async Task<ActionResult> SingleLogoutService()
        {
            // Receive the single logout request or response.
            // If a request is received then single logout is being initiated by the identity provider.
            // If a response is received then this is in response to single logout having been initiated by the service provider.
            var sloResult = await _samlServiceProvider.ReceiveSloAsync();

            if (sloResult.IsResponse)
            {
                // SP-initiated SLO has completed.
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Logout locally.
                await _signInManager.SignOutAsync();

                // Respond to the IdP-initiated SLO request indicating successful logout.
                await _samlServiceProvider.SendSloAsync();
            }

            return new EmptyResult();
        }
    }
}
