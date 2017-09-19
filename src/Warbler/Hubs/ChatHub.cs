using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Warbler.Misc;
using Warbler.Models;
using Warbler.Services;

namespace Warbler.Hubs
{
    /// <summary>
    ///   Coordinates websocket communication between
    ///   clients and server on the Chat view.
    /// </summary>
    public class ChatHub : Hub
    {
        private ChatService ChatService { get; }
        private UserManager<User> UserManager { get; }

        /// <summary>
        ///   Automatically called each time SignalR receives a packet from a client.
        /// </summary>
        public ChatHub(UserManager<User> userManager, WarblerDbContext context, ChatService service)
        {
            ChatService = service.With(context);
            UserManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
                await ChatService.OnConnectedAsync(user, Context.ConnectionId);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(
                    "Weird SignalR bug that happens sometimes. Fixing soon ~BC", ex);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
            await ChatService.OnDisconnectedAsync(user, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
