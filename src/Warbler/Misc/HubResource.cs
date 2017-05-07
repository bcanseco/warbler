using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Warbler.Models;

namespace Warbler.Misc
{
    /// <summary>
    ///   Parent class for services that directly interoperate with hubs.
    ///   Provides lazy-loading support (convention-friendly singleton).
    /// </summary>
    /// <typeparam name="TService">The service subclass.</typeparam>
    /// <typeparam name="THub">The hub being serviced.</typeparam>
    public abstract class HubResource<TService, THub>
        where TService : new()
        where THub : Hub
    {
        private static readonly Lazy<TService> Resource =
            new Lazy<TService>(() => new TService());

        /// <summary> Gets the singleton instance of the service. </summary>
        public static TService Instance => Resource.Value;

        /// <summary> Allows hub services to access the hub's clients/groups. </summary>
        protected IHubContext HubContext { get; }

        /// <summary> Tracks all online users on all of their devices. </summary>
        private ConnectionMapping UserConnections { get; } = new ConnectionMapping();

        protected HubResource()
        {
            HubContext = Startup.ConnectionManager.GetHubContext<THub>();
        }

        /// <summary>
        ///   Associates the user with the connection ID. Returns <see langword="true"/>
        ///   if the user wasn't previously connected to the hub on any device.
        /// </summary>
        /// <param name="user">The user to add or update.</param>
        /// <param name="connectionId">The connection ID to associate.</param>
        /// <remarks>
        ///   True if only one connection ID is now associated with the user, false otherwise.
        /// </remarks>
        /// <remarks>Call this at the beginning of overriden child OnConnected().</remarks>
        protected async Task<bool> OnConnected(User user, string connectionId)
            => await UserConnections.Add(user, connectionId);

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
        protected async Task<bool> OnDisconnected(User user, string connectionId)
            => await UserConnections.Remove(user, connectionId);
    }
}
