using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Warbler.Models;

namespace Warbler.Misc
{
    /// <summary>
    ///   Parent class for services that directly interoperate with hubs.
    /// </summary>
    /// <typeparam name="THub">The hub being serviced.</typeparam>
    public abstract class HubResource<THub>
        where THub : Hub
    {
        /// <summary> Allows hub services to access the hub's clients/groups. </summary>
        protected IHubContext<THub> HubContext { get; }

        /// <summary> Keeps track of all online users on all of their devices. </summary>
        private ConnectionMapping UserConnections { get; }

        protected HubResource(IHubContext<THub> hubContext, ILogger logger)
            => (HubContext, UserConnections) = (hubContext, new ConnectionMapping(logger));

        /// <summary>
        ///   Associates the user with the connection ID. Returns <see langword="true"/>
        ///   if the user wasn't previously connected to the hub on any device.
        /// </summary>
        /// <param name="user">The user to add or update.</param>
        /// <param name="connectionId">The connection ID to associate.</param>
        /// <returns>
        ///   True if only one connection ID is now associated with the user, false otherwise.
        /// </returns>
        /// <remarks>Call this at the beginning of overriden child OnConnected().</remarks>
        protected async Task<bool> OnConnectedAsync(User user, string connectionId)
            => await UserConnections.AddAsync(user, connectionId);

        /// <summary>
        ///   Disassociates the user with the connection ID. Returns <see langword="true"/>
        ///   if the user is no longer connected to the hub on any device.
        /// </summary>
        /// <param name="user">The user to remove or update.</param>
        /// <param name="connectionId">The connection ID to disassociate.</param>
        /// <returns>
        ///   True if no other connection IDs are associated with the user, false otherwise.
        /// </returns>
        /// <remarks>Call this at the beginning of overriden child OnDisconnected().</remarks>
        protected async Task<bool> OnDisconnectedAsync(User user, string connectionId)
            => await UserConnections.RemoveAsync(user, connectionId);
    }
}
