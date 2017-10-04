using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Misc;
using Warbler.Repositories;
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
        private UserService UserService { get; }

        /// <summary>
        ///   Automatically called each time SignalR receives a packet from a client.
        /// </summary>
        public ChatHub(WarblerDbContext context, ChatService service)
        {
            ChatService = service.With(context);
            UserService = new UserService(new SqlUserRepository(context));
        }

        public override async Task OnConnectedAsync()
        {
            var user = await UserService.FindByNameAsync(Context.User.Identity.Name);
            await ChatService.OnConnectedAsync(user, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await UserService.FindByNameAsync(Context.User.Identity.Name);
            await ChatService.OnDisconnectedAsync(user, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
