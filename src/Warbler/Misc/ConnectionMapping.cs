using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Misc
{
    public class ConnectionMapping
    {
        private ConcurrentDictionary<User, HashSet<string>> UserConnections { get; }
            = new ConcurrentDictionary<User, HashSet<string>>();

        public Task<bool> Add(User user, string connectionId)
        {
            var retVal = false;
            if (!UserConnections.TryGetValue(user, out HashSet<string> connections))
            {
                connections = new HashSet<string>();
                UserConnections.TryAdd(user, connections);
                retVal = true;
            }
            connections.Add(connectionId);
            return Task.FromResult(retVal);
        }

        public IEnumerable<string> GetConnections(User user)
        {
            UserConnections.TryGetValue(user, out HashSet<string> connections);
            return connections ?? Enumerable.Empty<string>();
        }

        public Task<bool> Remove(User user, string connectionId)
        {
            UserConnections.TryGetValue(user, out HashSet<string> connections);

            connections?.Remove(connectionId);

            if (connections?.Count == 0)
            {
                UserConnections.TryRemove(user, out _);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
