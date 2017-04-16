using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Areas.Chat.Hubs;

namespace Warbler.Areas.Chat.HubResources
{
    public class ChatResource
    {
        private static readonly Lazy<ChatResource> Resource =
            new Lazy<ChatResource>(
                () => new ChatResource(
                    Startup.ConnectionManager.GetHubContext<ChatHub>()));

        public static ChatResource Instance => Resource.Value;
        private static ConcurrentDictionary<string, string> ConnectedClients { get; set; }
        private IHubContext Context { get; }

        private ChatResource(IHubContext context)
        {
            Context = context;
            ConnectedClients = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        ///   Invoked from the hub on each connected client.
        /// </summary>
        /// <param name="connectionId">The client's uuid.</param>
        /// <param name="userName">The client's username, or null if not logged in</param>
        public Task OnConnected(string connectionId, string userName)
        {
            ConnectedClients.GetOrAdd(connectionId, userName);
            Context.Clients.All.updatedClientCount(ConnectedClients.Count);

            return Task.CompletedTask;
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
