using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Warbler.Areas.Chat.Hubs;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Services;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.HubResources
{
    public class ProximityResource
    {
        private static readonly Lazy<ProximityResource> Resource =
            new Lazy<ProximityResource>(
                () => new ProximityResource(
                    Startup.ConnectionManager.GetHubContext<ProximityHub>()));

        public static ProximityResource Instance => Resource.Value;
        private static ConcurrentDictionary<string, List<NearByResult>> UserChoices { get; }
            = new ConcurrentDictionary<string, List<NearByResult>>();

        private IHubContext Context { get; }
        private UniversityService UniversityService { get; set; }

        private ProximityResource(IHubContext context)
        {
            Context = context;
        }

        /// <summary>
        ///   TODO: Use singleton DI
        /// </summary>
        public Task Attach(WarblerDbContext context)
        {
            UniversityService = UniversityService ?? new UniversityService(context);
            return Task.CompletedTask;
        }

        /// <summary>
        ///   Invoked from the hub on each connected client.
        /// </summary>
        /// <param name="identifier">
        ///   The client's username, or connection string if not logged in.
        /// </param>
        public Task OnConnected(string identifier)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///   Invoked from the hub on each disconnected client.
        /// </summary>
        /// <param name="connectionId">The client's uuid.</param>
        public Task OnDisconnected(string connectionId)
        {
            UserChoices.TryRemove(connectionId, out List<NearByResult> us);
            return Task.CompletedTask;
        }

        public async Task GetNearbyUniversities(
            string identifier,
            Location coordinates)
        {
            List<NearByResult> nearbyUniversities = null;
            try
            {
                nearbyUniversities = await ProximityService
                    .UniversitiesNear(coordinates.Lat, coordinates.Lng);
            }
            catch (Exception ex)
            {
                var k = ex;
            }
            UserChoices.TryAdd(identifier, nearbyUniversities);
            Context.Clients.Client(identifier).receiveNearbyUniversities(nearbyUniversities);
        }

        public async Task SelectUniversity(string identifier, string placeId)
        {
            if (UserChoices.TryGetValue(identifier, out List<NearByResult> validChoices))
            {
                var chosenUni =
                    validChoices
                        .FirstOrDefault(u => u.PlaceId == placeId)
                    ?? throw new Exception($"{identifier} injected javascript; invalid choice.");
                await UniversityService.AddUniversity(chosenUni);
            }
            else
            {
                throw new ArgumentException($"{identifier} has not searched for universities.");
            }
        }
    }
}
