using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
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

        private UniversityService UniversityService { get; set; }
        private ILogger Logger { get; }

        public ProximityService With(WarblerDbContext context)
        {
            UniversityService = new UniversityService(new SqlUniversityRepository(context));
            return this;
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

            // Broadcast the list to the client for selection
            await HubContext.Clients.Client(connectionId)
                .InvokeAsync("receiveNearbyUniversities", nearbyUniversities);

            Logger.LogInformation($"{user.UserName} ({connectionId}) was " +
                            $"sent {nearbyUniversities.Count} universities.");
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
                .Where(u => (u.Is(PlaceLocationType.University) || u.Is(PlaceLocationType.Library))
                            && !(!u.Is(PlaceLocationType.University) && u.Is(PlaceLocationType.School)))
                .Where(u => !u.IsDepartment())
                .Where(u => u.Rating > 0.0)
                .OrderBy(u => u.Name)
                .ToList();
        }
    }
}
