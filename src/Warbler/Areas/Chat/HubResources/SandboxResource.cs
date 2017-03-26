using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Areas.Chat.Hubs;
using Warbler.Infrastructure.Chat.Services;

namespace Warbler.Areas.Chat.HubResources
{
    public class SandboxResource
    {
        private static readonly Lazy<SandboxResource> Resource =
            new Lazy<SandboxResource>(
                () => new SandboxResource(
                    Startup.ConnectionManager.GetHubContext<SandboxHub>(),
                    new SandboxService()));

        private static ConcurrentDictionary<string, string> ConnectedClients { get; set; }
        private IHubContext Context { get; }
        private SandboxService SandboxService { get; }

        private SandboxResource(IHubContext context, SandboxService sandboxService)
        {
            Context = context;
            SandboxService = sandboxService;
            ConnectedClients = new ConcurrentDictionary<string, string>();
        }

        public static SandboxResource Instance => Resource.Value;

        /// <summary>
        ///   Invoked from the hub on each connected client.
        /// </summary>
        /// <param name="connectionId">The client's uuid.</param>
        /// <param name="userName">The client's username, or null if not logged in</param>
        public async Task OnConnected(string connectionId, string userName)
        {
            ConnectedClients.GetOrAdd(connectionId, userName);
            Context.Clients.All.updatedClientCount(ConnectedClients.Count);

            await SandboxService.GetUniversities(); // TODO: Testing; remove
        }

        /// <summary>
        ///   Invoked from the hub on each disconnected client.
        /// </summary>
        /// <param name="connectionId">The client's uuid.</param>
        public void OnDisconnected(string connectionId)
        {
            ConnectedClients.TryRemove(connectionId, out string removedUser);
            Context.Clients.All.updatedClientCount(ConnectedClients.Count);
        }

        public void OnMessageReceived(string message)
            => Context.Clients.All.messageReceived($"[{DateTime.Now}] {message}");
    }
}
