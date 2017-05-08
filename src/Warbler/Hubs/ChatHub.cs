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
        public ChatHub(UserManager<User> userManager, WarblerDbContext dbContext)
        {
            ChatService = ChatService.Instance.With(dbContext);
            UserManager = userManager;
        }

        public override async Task OnConnected()
        {
            var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
            await ChatService.OnConnected(user, Context.ConnectionId);

            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
            await ChatService.OnDisconnected(user, Context.ConnectionId);

            await base.OnDisconnected(stopCalled);
        }
    }
}
