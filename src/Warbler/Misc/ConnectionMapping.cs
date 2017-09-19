using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Warbler.Models;

namespace Warbler.Misc
{
    public class ConnectionMapping
    {
        public ConnectionMapping(ILogger logger)
            => Logger = logger;

        private ILogger Logger { get; }
        private ConcurrentDictionary<User, HashSet<string>> UserConnections { get; }
            = new ConcurrentDictionary<User, HashSet<string>>();

        public Task<bool> AddAsync(User user, string connectionId)
        {
            var retVal = false;

            if (!UserConnections.TryGetValue(user, out var connections))
            {
                Logger.LogInformation($"Adding {user.UserName} ({connectionId}) with no previous connections.");
                connections = new HashSet<string>();
                UserConnections.TryAdd(user, connections);
                retVal = true;
            }
            connections.Add(connectionId);
            Logger.LogInformation($"{user.UserName} ({connectionId}) was added to the hub [{connections.Count}].");
            return Task.FromResult(retVal);
        }

        public IEnumerable<string> GetConnections(User user)
        {
            UserConnections.TryGetValue(user, out var connections);
            return connections ?? Enumerable.Empty<string>();
        }

        public Task<bool> RemoveAsync(User user, string connectionId)
        {
            UserConnections.TryGetValue(user, out var connections);
            connections?.Remove(connectionId);

            if (connections?.Count == 0)
            {
                UserConnections.TryRemove(user, out _);
                Logger.LogInformation($"Removing {user.UserName} with no further connections.");
                return Task.FromResult(true);
            }
            Logger.LogInformation($"{user.UserName} was removed from the hub [{connections?.Count}].");
            return Task.FromResult(false);
        }
    }
}
