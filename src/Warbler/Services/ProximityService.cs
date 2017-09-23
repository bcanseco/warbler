using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Common.Enums;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Warbler.Extensions;
using Warbler.Hubs;
using Warbler.Models;
using Warbler.Misc;
using Warbler.Repositories;
using static GoogleApi.Entities.Places.Common.Enums.PlaceLocationType;

namespace Warbler.Services
{
    /// <summary>
    ///   Used by the <see cref="ProximityHub"/> for business logic.
    /// </summary>
    public class ProximityService : HubResource<ProximityHub>
    {
        /// <summary>
        ///   Allows querying the Google Places API.
        /// </summary>
        private readonly string _googleApiKey;

        public ProximityService(
            IHubContext<ProximityHub> hubContext,
            IOptions<ApiKeys> apiKeys,
            ILogger<ProximityService> logger) : base(hubContext, logger)
        {
            
            _googleApiKey = apiKeys.Value.GooglePlaces;
            Logger = logger;
        }

        /// <summary> Stores proximity search results for each user. </summary>
        private ConcurrentDictionary<string, List<NearByResult>> UserChoices { get; }
            = new ConcurrentDictionary<string, List<NearByResult>>();

        private UniversityService UniversityService { get; set; }
        private ILogger Logger { get; }

        public ProximityService With(WarblerDbContext context)
        {
            UniversityService = new UniversityService(new SqlUniversityRepository(context));
            return this;
        }

        /// <summary>
        ///   Invoked from the hub on each disconnected client. Removes the
        ///   client's choice list from <see cref="UserChoices"/> if it exists.
        /// </summary>
        public Task OnDisconnectedAsync(string userName)
        {
            UserChoices.TryRemove(userName, out var _);
            Logger.LogInformation($"{userName} was disconnected.");
            return Task.CompletedTask;
        }

        /// <summary>
        ///   Conducts a proximity search and returns the filtered results.
        /// </summary>
        /// <param name="user">The user to associate the results with.</param>
        /// <param name="connectionId">The connection ID of the user.</param>
        /// <param name="coordinates">The location to search near.</param>
        public async Task ProximitySearchAsync(
            User user, string connectionId, Location coordinates)
        {
            var nearbyUniversities = await SearchUniversitiesAsync(coordinates);

            // Keep track of the search results for later validation
            UserChoices.TryAdd(user.UserName, nearbyUniversities);

            // Broadcast the list to the client for selection
            await HubContext.Clients.Client(connectionId)
                .InvokeAsync("receiveNearbyUniversities", nearbyUniversities);

            Logger.LogInformation($"{user.UserName} ({connectionId}) was " +
                            $"sent {nearbyUniversities.Count} universities.");
        }

        /// <summary>
        ///   Validates the user's university selection and lets them join if successful.
        /// </summary>
        /// <param name="user">The user selecting a university.</param>
        /// <param name="connectionId">The user's connection ID.</param>
        /// <param name="placeId">The Google Place ID of the selected university.</param>
        /// <exception cref="System.NullReferenceException">
        ///   Thrown if this method is called for a user before <see cref="ProximitySearchAsync"/>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        ///   Thrown if <paramref name="placeId"/> was not yielded for this user's proximity search.
        /// </exception>
        public async Task SelectUniversityAsync(User user, string connectionId, string placeId)
        {
            // Grab the list of universities previously presented to this User
            UserChoices.TryGetValue(user.UserName, out var validChoices);

            // Verify that the given place ID is among those list of universities
            var userChoice =
                validChoices?.SingleOrDefault(u => u.PlaceId == placeId)
                ?? throw new InvalidDataException("Invalid choice; user may have injected JS.");

            var university = await UniversityService.GetOrCreateAsync(userChoice);
            await UniversityService.JoinAsync(user, university);

            // Let client know they can request the chatroom view now
            await HubContext.Clients.Client(connectionId).InvokeAsync("successfulJoin");

            Logger.LogInformation($"{user.UserName} successfully joined {university.Name}.");
        }

        /// <summary>
        ///   Returns a list of Google Places result objects
        ///   that are geographically close to a given location.
        /// </summary>
        /// <param name="location">Contains lat/lng used for the location search.</param>
        private async Task<List<NearByResult>> SearchUniversitiesAsync(Location location)
        {
            var request = new PlacesNearBySearchRequest
            {
                Keyword = "university",
                Location = location,
                Radius = 25000,
                Key = _googleApiKey
            };

            var response = await GooglePlaces.NearBySearch.QueryAsync(request);

            // Some results are mistagged; this filter hopefully reduces misses/false-positives
            return response.Results
                .Where(u => (u.Is(PlaceLocationType.University) || u.Is(Library))
                            && !(!u.Is(PlaceLocationType.University) && u.Is(School)))
                .Where(u => !u.IsDepartment())
                .Where(u => u.Rating > 0.0)
                .OrderBy(u => u.Name)
                .ToList();
        }
    }
}
