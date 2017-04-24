using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Warbler.Areas.Chat.HubResources;
using Warbler.Areas.Chat.Models;
using Warbler.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Warbler.Areas.Chat.Hubs
{
    public class ProximityHub : Hub
    {
        private ProximityResource UniversitiesResource { get; }

        private ProximityHub(ProximityResource universitiesResource, WarblerDbContext context)
        {
            UniversitiesResource = universitiesResource;
            UniversitiesResource.Attach(context);
        }

        public ProximityHub(WarblerDbContext context)
            : this(ProximityResource.Instance, context)
        { }

        public override async Task OnConnected()
        {
            await UniversitiesResource
                .OnConnected(Context.User.Identity.IsAuthenticated
                    ? Context.User.Identity.Name
                    : Context.ConnectionId);

            await base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UniversitiesResource.OnDisconnected(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public async Task GetNearbyUniversities(string serializedLatLng)
        {
            var coordinates = JsonConvert
                .DeserializeObject<Location>(serializedLatLng);

            await UniversitiesResource
                .GetNearbyUniversities(Context.ConnectionId, coordinates);
        }

        public async Task SelectUniversity(string placeId)
        {
            await UniversitiesResource
                .SelectUniversity(Context.ConnectionId, placeId);
        }
    }
}
