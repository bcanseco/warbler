using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Areas.Chat.HubResources;

namespace Warbler.Areas.Chat.Hubs
{
    public class ChatHub : Hub
    {
        private ChatResource ChatResource { get; }

        private ChatHub(ChatResource chatResource)
        {
            ChatResource = chatResource;
        }

        public ChatHub()
            : this(ChatResource.Instance)
        { }

        public override async Task OnConnected()
        {
            await ChatResource.OnConnected(Context.ConnectionId,
                Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : null);

            await base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            ChatResource.OnDisconnected(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public void SendMessage(string message)
            => ChatResource.OnMessageReceived(message);
    }
}
