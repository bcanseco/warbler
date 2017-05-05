using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Hubs;
using Warbler.Areas.Chat.Models;

namespace Warbler.Areas.Chat.Extensions
{
    public static class UserManagerExtensions
    {
        /// <summary>
        ///   Gets the <see cref="User"/> connected to the hub and returns
        ///   the object after attaching its corresponding connection ID.
        /// </summary>
        public static async Task<User> FindWithHubAsync(
            this UserManager<User> userManager,
            HubCallerContext context)
        {
            var foundUser = await userManager.FindByNameAsync(context.User.Identity.Name);
            foundUser.ConnectionId = context.ConnectionId;
            return foundUser;
        }
    }
}
