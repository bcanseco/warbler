using System.Collections.Concurrent;
using System.Threading.Tasks;
using Warbler.Misc;

namespace Warbler.Services
{
    /// <summary>
    ///   Used by the <see cref="Hubs.ChatHub"/> for business logic.
    /// </summary>
    public class ChatService : HubResource<ChatService>
    {
        private ConcurrentDictionary<string, string> ConnectedClients { get; }
            = new ConcurrentDictionary<string, string>();

        /// <summary>
        ///   Invoked from the hub on each connected client.
        /// </summary>
        public Task OnConnected(string connectionId, string userName)
        {
            ConnectedClients.GetOrAdd(connectionId, userName);
            return Task.CompletedTask;
        }

        /// <summary>
        ///   Invoked from the hub on each disconnected client.
        /// </summary>
        public Task OnDisconnected(string connectionId)
        {
            ConnectedClients.TryRemove(connectionId, out string _);
            return Task.CompletedTask;
        }
    }
}
