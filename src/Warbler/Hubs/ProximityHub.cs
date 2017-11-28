using System;
using System.Threading.Tasks;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Warbler.Misc;
using Warbler.Repositories;
using Warbler.Services;

namespace Warbler.Hubs
{
    /// <summary>
    ///   Coordinates websocket communication between
    ///   clients and server on the proximity component view.
    /// </summary>
    public class ProximityHub : Hub
    {
        private ProximityService ProximityService { get; }
        private UserService UserService { get; }

        /// <summary>
        ///   Automatically called each time SignalR receives a packet from a client.
        ///   Both parameters are injected automatically by ASP.NET DI.
        /// </summary>
        public ProximityHub(ProximityService service, WarblerDbContext context)
        {
            ProximityService = service.With(context);
            UserService = new UserService(new SqlUserRepository(context));
        }
        
        /// <summary>
        ///   Called via SignalR when the user enters the Chat view
        ///   without being a member of any universities.
        /// </summary>
        /// <param name="locationSer">The serialized location (lat/lng).</param>
        public async Task GetNearbyUniversitiesAsync(string locationSer)
        { 
            var user = await UserService.FindByNameAsync(Context.User.Identity.Name);
            var coordinates = JsonConvert.DeserializeObject<Location>(locationSer);

            await ProximityService.ProximitySearchAsync(user, Context.ConnectionId, coordinates);
        }
    }
}
