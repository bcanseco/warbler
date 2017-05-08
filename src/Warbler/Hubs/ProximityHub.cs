using System.Threading.Tasks;
using GoogleApi.Entities.Common;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Warbler.Models;
using Warbler.Misc;
using Microsoft.AspNetCore.Identity;
using Warbler.Services;

namespace Warbler.Hubs
{
    /// <summary>
    ///   Coordinates websocket communication between
    ///   clients and server on the chat index view.
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

        public override async Task OnDisconnected(bool stopCalled)
        {
            var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
            await ProximityService.OnDisconnected(user);

            await base.OnDisconnected(stopCalled);
        }
        
        /// <summary>
        ///   Called via SignalR when the user enters the Chat view
        ///   without being a member of any universities.
        /// </summary>
        /// <param name="locationSer">The serialized location (lat/lng).</param>
        public async Task GetNearbyUniversitiesAsync(string locationSer)
        { 
            var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
            var coordinates = JsonConvert.DeserializeObject<Location>(locationSer);

            await ProximityService
                .ProximitySearchAsync(user, Context.ConnectionId, coordinates);
        }

        /// <summary>
        ///   Called via SignalR when the user clicks on a university to connect to.
        /// </summary>
        /// <param name="placeId">The Google Place ID of the clicked university.</param>
        public async Task SelectUniversityAsync(string placeId)
        {
            var user = await UserManager.FindByNameAsync(Context.User.Identity.Name);
            await ProximityService.SelectUniversityAsync(user, Context.ConnectionId, placeId);
        }
    }
}
