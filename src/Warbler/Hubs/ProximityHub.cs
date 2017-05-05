using System.Threading.Tasks;
using GoogleApi.Entities.Common;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Warbler.Models;
using Warbler.Misc;
using Microsoft.AspNetCore.Identity;
using Warbler.Extensions;
using Warbler.Services;

namespace Warbler.Hubs
{
    /// <summary>
    ///   Coordinates websocket communication between clients and server.
    /// </summary>
    public class ProximityHub : Hub
    {
        private ProximityService ProximityService { get; }
        private UserManager<User> UserManager { get; }

        /// <summary>
        ///   Automatically called each time SignalR receives a packet from a client.
        ///   Both parameters are injected automatically by ASP.NET DI.
        /// </summary>
        public ProximityHub(WarblerDbContext context, UserManager<User> userManager)
        {
            ProximityService = ProximityService.Instance.With(context);
            UserManager = userManager;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            ProximityService.OnDisconnected(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        
        /// <summary>
        ///   Called via SignalR when the user enters the Chat view
        ///   without being a member of any universities.
        /// </summary>
        public async Task GetNearbyUniversitiesAsync(string serializedLatLng)
        { 
            var currentUser = await UserManager.FindWithHubAsync(Context);

            var coordinates = JsonConvert
                .DeserializeObject<Location>(serializedLatLng);

            var nearbyUniversities = await ProximityService
                .ProximitySearchAsync(currentUser, coordinates);

            // Broadcast the list to the client for selection
            Clients.Client(currentUser.ConnectionId)
                .receiveNearbyUniversities(nearbyUniversities);
        }

        /// <summary>
        ///   Called via SignalR when the user clicks on a university to connect to.
        /// </summary>
        public async Task SelectUniversityAsync(string placeId)
            => await ProximityService
                .SelectUniversityAsync(Context.User.Identity.Name, placeId);
    }
}
