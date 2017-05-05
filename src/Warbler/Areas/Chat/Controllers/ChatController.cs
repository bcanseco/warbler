﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Repositories;
using Warbler.Areas.Chat.Services;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.Controllers
{
    [Area("Chat")]
    [Authorize]
    public class ChatController : Controller
    {
        private UserManager<User> UserManager { get; }
        private UserService UserService { get; }

        public ChatController(UserManager<User> userManager, WarblerDbContext context)
        {
            UserManager = userManager;
            UserService = new UserService(new SqlUserRepository(context));
        }

        /// <summary>
        ///   Resolves the Index view if the user isn't a member of any
        ///   universities, otherwises redirects to <see cref="Chatroom"/>.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var user = await UserManager.GetUserAsync(User);
            var noMemberships = await UserService.IsNewAsync(user);

            if (noMemberships) return View();
            return RedirectToAction(nameof(Chatroom));
        }
        
        public IActionResult Chatroom()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
