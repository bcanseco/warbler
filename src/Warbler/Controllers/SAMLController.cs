using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ComponentSpace.Saml2;
using ComponentSpace.Saml2.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warbler.Models;

namespace Warbler.Controllers
{
    public class SamlController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ISamlServiceProvider _samlServiceProvider;

        public SamlController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ISamlServiceProvider samlServiceProvider/*,
            SamlConfigurations configs*/)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _samlServiceProvider = samlServiceProvider;
        }

        public async Task<IActionResult> AssertionConsumerService()
        {
            // Receive and process the SAML assertion contained in the SAML response.
            // The SAML response is received either as part of IdP-initiated or SP-initiated SSO.
            var ssoResult = await _samlServiceProvider.ReceiveSsoAsync();

            // Automatically provision the user.
            // If the user doesn't exist locally then create the user.
            // Automatic provisioning is an optional step.
            var user = await _userManager.FindByNameAsync(ssoResult.UserID);

            if (user == null)
            {
                var email = ssoResult.Attributes.First(a => a.Name == ClaimTypes.Email).ToString();
                user = new User { UserName = ssoResult.UserID, Email = email };
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    throw new Exception($"The user {ssoResult.UserID} couldn't be created - {result}");
                }

                // For demonstration purposes, create some additional claims.
                if (ssoResult.Attributes != null)
                {
                    var samlAttribute = ssoResult.Attributes.SingleOrDefault(a => a.Name == ClaimTypes.GivenName);

                    if (samlAttribute != null)
                    {
                        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, samlAttribute.ToString()));
                    }

                    samlAttribute = ssoResult.Attributes.SingleOrDefault(a => a.Name == ClaimTypes.Surname);

                    if (samlAttribute != null)
                    {
                        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Surname, samlAttribute.ToString()));
                    }
                }
            }

            // Automatically login using the asserted identity.
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect to the target URL if specified and this is IdP-initiated SSO.
            if (!ssoResult.IsInResponseTo && !string.IsNullOrEmpty(ssoResult.RelayState))
            {
                return Redirect(ssoResult.RelayState);
            }

            return RedirectToAction("Index", "Home");
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
