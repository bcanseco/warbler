using System;
using System.Threading.Tasks;
using GoogleApi.Entities.Common;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Warbler.Models;
using Microsoft.AspNetCore.Identity;
using Warbler.Misc;
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
        public ProximityHub(ProximityService service, WarblerDbContext context, UserManager<User> userManager)
        {
            ProximityService = service.With(context);
            UserManager = userManager;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await ProximityService.OnDisconnectedAsync(Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
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

            await ProximityService.ProximitySearchAsync(user, Context.ConnectionId, coordinates);
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
