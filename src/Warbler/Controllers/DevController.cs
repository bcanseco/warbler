﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Warbler.Repositories;
using Warbler.Services;
using Warbler.Misc;

namespace Warbler.Controllers
{
    public class DevController : Controller
    {
        private UniversityService UniversityService { get; }

        public DevController(WarblerDbContext context)
        {
            UniversityService = new UniversityService(new SqlUniversityRepository(context));
        }

        [Authorize]
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
    }
}
