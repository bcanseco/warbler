﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.Controllers
{
    [Area("Chat")]
    // [Authorize]
    public class ChatController : Controller
    {
        private WarblerDbContext Context { get; }

        public ChatController(WarblerDbContext context)
        {
            Context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Chat()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}