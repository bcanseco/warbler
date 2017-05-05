using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Areas.Chat.Services;

namespace Warbler.Areas.Chat.Hubs
{
    /// <summary>
    ///   Coordinates websocket communication between clients and server.
    /// </summary>
    public class ChatHub : Hub
    {
        private ChatService ChatService { get; }

        /// <summary>
        ///   Automatically called each time SignalR receives a packet from a client.
        /// </summary>
        public ChatHub()
        {
            ChatService = ChatService.Instance;
        }

        public override async Task OnConnected()
        {
            await ChatService.OnConnected(Context.ConnectionId, null);
            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await ChatService.OnDisconnected(Context.ConnectionId);
            await base.OnDisconnected(stopCalled);
        }

        /// <summary>
        ///   Called via SignalR when the user hits the Send button.
        /// </summary>
        public Task SendMessage(string message)
            => Clients.All.receiveMessage($"[{DateTime.Now}] {message}");
    }
}
