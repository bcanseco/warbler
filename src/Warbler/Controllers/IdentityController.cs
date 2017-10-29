/*using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Warbler.Controllers
{
    [Produces("application/json")]
    [Route("api/Identity")]
    public class IdentityController : Controller
    {
        [HttpGet]
        public IActionResult Get() => new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }
}*/
