using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Areas.Chat.HubResources;

namespace Warbler.Areas.Chat.Hubs
{
    public class SandboxHub : Hub
    {
        private SandboxResource SandboxResource { get; }

        private SandboxHub(SandboxResource sandboxResource)
        {
            SandboxResource = sandboxResource;
        }

        public SandboxHub() :
            this(SandboxResource.Instance)
        { }

        public override async Task OnConnected()
        {
            await SandboxResource.OnConnected(Context.ConnectionId,
                Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : null);

            await base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            SandboxResource.OnDisconnected(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public void SendMessage(string message)
            => SandboxResource.OnMessageReceived(message);
    }
}
