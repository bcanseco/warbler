using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Warbler.Repositories;
using Warbler.Services;
using Warbler.Misc;
using Warbler.Models;

namespace Warbler.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DevController : Controller
    {
        private UniversityService UniversityService { get; }
        private ClaimRequestService ClaimRequestService { get; }

        public DevController(WarblerDbContext context)
        {
            UniversityService = new UniversityService(new SqlUniversityRepository(context));
            ClaimRequestService = new ClaimRequestService(new SqlClaimRequestRepository(context));
        }
        
        public IActionResult Index()
        {
            ViewData["Title"] = "Developer Panel";
            ViewData["Heading"] = true;
            ViewData["Component"] = "Dev"; // Components/Dev

            return View("ReactComponent", ViewData);
        }

        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        ///   Returns a JSON string with all university info down to the message level.
        /// </summary>
        [HttpGet]
        public async Task<string> Universities()
        {
            var universityList = await UniversityService.GetAllAsync();
            return JsonConvert.SerializeObject(universityList, Formatting.Indented);
        }

        [HttpGet]
        public async Task<string> Claims()
        {
            var claimsList = await ClaimRequestService.GetAllUnresolvedAsync();
            return JsonConvert.SerializeObject(claimsList);
        }

        [HttpPost]
        public async Task ResolveClaim([FromBody] ClaimRequest claim)
        {
            if (claim.IsAccepted ?? false)
            {
                await UniversityService.ApplyClaimAsync(claim.University, claim.SubmitterId);
            }
            
            await ClaimRequestService.UpdateAsync(claim);
        }

        /// <summary>
        ///   TODO: For demo purposes only; remove this route.
        /// </summary>
        [Route("delete/{universityId:int}")]
        public async Task<IActionResult> DeleteUniversityAsync(int universityId)
        {
            await UniversityService.DeleteAsync(await UniversityService.FindByIdAsync(universityId));
            return RedirectToAction("Index");
        }
    }
}
